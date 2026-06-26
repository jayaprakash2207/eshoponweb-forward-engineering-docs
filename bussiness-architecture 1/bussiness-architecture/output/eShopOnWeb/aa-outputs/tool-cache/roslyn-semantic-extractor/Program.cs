
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

public sealed class SourceFileRecord
{
    public string Path { get; set; } = "";
    public string Project { get; set; } = "unknown";
    public string ProjectPath { get; set; } = "unknown";
    public string ProjectType { get; set; } = "unknown";
    public string ProjectCategory { get; set; } = "unknown";
    public List<string> ProjectReferences { get; set; } = new();
}

public static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: RoslynSemanticExtractor <repoRoot> <outputJson> <sourceListJson>");
            return 2;
        }

        var repoRoot = Path.GetFullPath(args[0]);
        var outputJson = Path.GetFullPath(args[1]);
        var sourceListJson = Path.GetFullPath(args[2]);
        var sourceRows = JsonSerializer.Deserialize<List<SourceFileRecord>>(
            File.ReadAllText(sourceListJson, Encoding.UTF8),
            JsonOptions) ?? new List<SourceFileRecord>();

        var allComponents = new List<Dictionary<string, object?>>();
        var allDependencies = new List<Dictionary<string, object?>>();
        var allRouteHints = new List<Dictionary<string, object?>>();
        var unresolvedInvocations = 0;
        var compilationDiagnostics = new List<Dictionary<string, object?>>();

        foreach (var projectGroup in sourceRows.GroupBy(row => row.Project).OrderBy(group => group.Key))
        {
            var primaryRows = projectGroup.ToList();
            var projectReferences = primaryRows.SelectMany(row => row.ProjectReferences).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().ToHashSet();
            var compilationRows = sourceRows
                .Where(row => row.Project == projectGroup.Key || (projectReferences.Contains(row.Project) && !row.Path.EndsWith("/Program.cs", StringComparison.OrdinalIgnoreCase)))
                .GroupBy(row => row.Path)
                .Select(group => group.First())
                .ToList();

            var sourceByPath = compilationRows.ToDictionary(row => NormalizePath(row.Path), row => row);
            var ownPaths = primaryRows.Select(row => NormalizePath(row.Path)).ToHashSet();
            var syntaxTrees = new List<SyntaxTree>();
            foreach (var row in compilationRows)
            {
                var absolutePath = Path.Combine(repoRoot, row.Path.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(absolutePath))
                {
                    continue;
                }

                var text = SourceText.From(File.ReadAllText(absolutePath, Encoding.UTF8), Encoding.UTF8);
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(
                    text,
                    new CSharpParseOptions(LanguageVersion.Preview, DocumentationMode.None, SourceCodeKind.Regular),
                    path: NormalizePath(row.Path)));
            }

            if (syntaxTrees.Count == 0)
            {
                continue;
            }

            var compilation = CSharpCompilation.Create(
                "ArchitectureSemanticExtraction_" + SafeId(projectGroup.Key),
                syntaxTrees,
                MetadataReferences(),
                new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    nullableContextOptions: NullableContextOptions.Enable));

            foreach (var diagnostic in compilation.GetDiagnostics().Where(item => item.Severity == DiagnosticSeverity.Error).Take(20))
            {
                compilationDiagnostics.Add(new Dictionary<string, object?>
                {
                    ["project"] = projectGroup.Key,
                    ["id"] = diagnostic.Id,
                    ["message"] = diagnostic.GetMessage(),
                    ["severity"] = diagnostic.Severity.ToString(),
                    ["source_file"] = NormalizePath(diagnostic.Location.GetLineSpan().Path),
                    ["line"] = diagnostic.Location.GetLineSpan().StartLinePosition.Line + 1
                });
            }

            var localTypesByFullName = new Dictionary<string, INamedTypeSymbol>();
            var localTypesBySimpleName = new Dictionary<string, List<INamedTypeSymbol>>();
            foreach (var tree in syntaxTrees)
            {
                var relPath = NormalizePath(tree.FilePath);
                var model = compilation.GetSemanticModel(tree, ignoreAccessibility: true);
                var root = tree.GetRoot();
                foreach (var typeDecl in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
                {
                    var symbol = model.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
                    if (symbol == null)
                    {
                        continue;
                    }

                    var fullName = CleanSymbol(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    localTypesByFullName[fullName] = symbol;
                    if (!localTypesBySimpleName.ContainsKey(symbol.Name))
                    {
                        localTypesBySimpleName[symbol.Name] = new List<INamedTypeSymbol>();
                    }
                    localTypesBySimpleName[symbol.Name].Add(symbol);
                }
            }

            foreach (var tree in syntaxTrees)
            {
                var relPath = NormalizePath(tree.FilePath);
                if (!ownPaths.Contains(relPath))
                {
                    continue;
                }

                var row = sourceByPath.TryGetValue(relPath, out var record) ? record : new SourceFileRecord { Path = relPath, Project = projectGroup.Key };
                var model = compilation.GetSemanticModel(tree, ignoreAccessibility: true);
                var root = tree.GetRoot();

                foreach (var typeDecl in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
                {
                    var symbol = model.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
                    if (symbol == null)
                    {
                        continue;
                    }

                    allComponents.Add(BuildComponent(symbol, typeDecl, tree, row));
                }

                foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    var syntaxMethodName = InvokedMethodName(invocation.Expression);
                    var callerType = invocation.Ancestors().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
                    var callerSymbol = callerType == null ? null : model.GetDeclaredSymbol(callerType) as INamedTypeSymbol;
                    var callerComponent = callerSymbol?.Name ?? (relPath.EndsWith("/Program.cs", StringComparison.OrdinalIgnoreCase) ? "Program" : "unknown");
                    var callerFullName = callerSymbol == null ? "Program" : CleanSymbol(callerSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    var callerMethod = ContainingMethodName(model, invocation);
                    var line = LineFor(tree, invocation);

                    if (IsDiRegistration(syntaxMethodName))
                    {
                        var registration = BuildDiRegistration(model, invocation, syntaxMethodName, callerComponent, relPath, line, row);
                        if (registration != null)
                        {
                            allDependencies.Add(registration);
                        }
                    }

                    var routeHint = BuildRouteHint(model, invocation, syntaxMethodName, callerComponent, relPath, line, row);
                    if (routeHint != null)
                    {
                        allRouteHints.Add(routeHint);
                    }

                    var methodSymbol = ResolveMethod(model, invocation);
                    if (methodSymbol == null)
                    {
                        unresolvedInvocations++;
                        continue;
                    }

                    var targetType = methodSymbol.ContainingType;
                    var targetTypeName = targetType?.Name ?? "unknown";
                    var targetFullName = targetType == null ? "unknown" : CleanSymbol(targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    var methodName = methodSymbol.Name;

                    var localTarget = localTypesByFullName.ContainsKey(targetFullName) || localTypesBySimpleName.ContainsKey(targetTypeName);
                    if (!localTarget || callerComponent == "unknown")
                    {
                        continue;
                    }

                    allDependencies.Add(new Dictionary<string, object?>
                    {
                        ["dependency_id"] = null,
                        ["kind"] = "roslyn_component_call",
                        ["from"] = callerComponent,
                        ["to"] = targetTypeName + "." + methodName,
                        ["relationship"] = "calls",
                        ["source_file"] = relPath,
                        ["line"] = line,
                        ["evidence"] = callerComponent + "." + callerMethod + " semantically resolves to " + targetFullName + "." + methodName + "()",
                        ["confidence"] = 0.94,
                        ["metadata"] = new Dictionary<string, object?>
                        {
                            ["parser_backend"] = "roslyn_semantic_model",
                            ["source_component"] = callerComponent,
                            ["source_symbol"] = callerFullName,
                            ["source_method"] = callerMethod,
                            ["target_component_or_type"] = targetTypeName,
                            ["target_symbol"] = targetFullName,
                            ["target_method"] = methodName,
                            ["resolution_quality"] = "roslyn_semantic_symbol_binding",
                            ["project"] = row.Project,
                            ["project_path"] = row.ProjectPath
                        }
                    });
                }
            }
        }

        var dedupedDependencies = DeduplicateDependencies(allDependencies)
            .Select((item, index) =>
            {
                item["dependency_id"] = "RDEP-" + (index + 1).ToString("D5");
                return item;
            })
            .ToList();

        var payload = new Dictionary<string, object?>
        {
            ["generated_at"] = DateTimeOffset.UtcNow.ToString("O"),
            ["extractor_version"] = "0.1.0",
            ["status"] = "active",
            ["parser_backend"] = "roslyn_semantic_model",
            ["summary"] = new Dictionary<string, object?>
            {
                ["source_file_count"] = sourceRows.Count,
                ["project_count"] = sourceRows.Select(row => row.Project).Distinct().Count(),
                ["semantic_component_count"] = allComponents.Count,
                ["semantic_dependency_count"] = dedupedDependencies.Count,
                ["semantic_component_call_count"] = dedupedDependencies.Count(item => Convert.ToString(item["kind"]) == "roslyn_component_call"),
                ["semantic_di_registration_count"] = dedupedDependencies.Count(item => Convert.ToString(item["kind"]) == "di_registration"),
                ["route_hint_count"] = allRouteHints.Count,
                ["unresolved_invocation_count"] = unresolvedInvocations,
                ["compilation_error_count_sampled"] = compilationDiagnostics.Count
            },
            ["semantic_components"] = allComponents,
            ["dependency_candidates"] = dedupedDependencies,
            ["route_hints"] = allRouteHints,
            ["diagnostics"] = compilationDiagnostics,
            ["limitations"] = new List<string>
            {
                "Roslyn semantic extraction compiles each project with project-reference source overlays where possible; package symbols may remain unresolved without restored package assemblies.",
                "Runtime dispatch, reflection, generated partial classes, and framework-generated endpoints still require runtime tracing or framework execution evidence."
            }
        };

        Directory.CreateDirectory(Path.GetDirectoryName(outputJson)!);
        File.WriteAllText(outputJson, JsonSerializer.Serialize(payload, JsonOptions) + Environment.NewLine, Encoding.UTF8);
        Console.WriteLine(JsonSerializer.Serialize(payload["summary"], JsonOptions));
        return 0;
    }

    private static Dictionary<string, object?> BuildComponent(INamedTypeSymbol symbol, BaseTypeDeclarationSyntax typeDecl, SyntaxTree tree, SourceFileRecord row)
    {
        var methods = new List<Dictionary<string, object?>>();
        foreach (var member in symbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (member.MethodKind != MethodKind.Ordinary)
            {
                continue;
            }
            if (member.DeclaredAccessibility != Accessibility.Public && member.DeclaredAccessibility != Accessibility.Internal)
            {
                continue;
            }
            methods.Add(new Dictionary<string, object?>
            {
                ["name"] = member.Name,
                ["return_type"] = CleanSymbol(member.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                ["parameters"] = member.Parameters.Select(parameter => new Dictionary<string, object?>
                {
                    ["name"] = parameter.Name,
                    ["type"] = CleanSymbol(parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                }).ToList()
            });
        }

        var constructorDependencies = symbol.Constructors
            .Where(ctor => ctor.Parameters.Length > 0)
            .SelectMany(ctor => ctor.Parameters.Select(parameter => new Dictionary<string, object?>
            {
                ["name"] = parameter.Name,
                ["type"] = CleanSymbol(parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                ["constructor"] = symbol.Name
            }))
            .ToList();

        return new Dictionary<string, object?>
        {
            ["name"] = symbol.Name,
            ["full_name"] = CleanSymbol(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
            ["namespace"] = symbol.ContainingNamespace?.IsGlobalNamespace == false ? symbol.ContainingNamespace.ToDisplayString() : null,
            ["kind"] = symbol.TypeKind.ToString(),
            ["file"] = NormalizePath(tree.FilePath),
            ["line"] = LineFor(tree, typeDecl),
            ["project"] = row.Project,
            ["project_type"] = row.ProjectType,
            ["project_category"] = row.ProjectCategory,
            ["base_type"] = symbol.BaseType == null || symbol.BaseType.SpecialType == SpecialType.System_Object ? null : CleanSymbol(symbol.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
            ["interfaces"] = symbol.Interfaces.Select(item => CleanSymbol(item.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))).ToList(),
            ["attributes"] = symbol.GetAttributes().Select(attr => attr.AttributeClass?.Name ?? attr.ToString()).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().ToList(),
            ["public_or_internal_methods"] = methods,
            ["constructor_dependencies"] = constructorDependencies,
            ["confidence"] = 0.96,
            ["semantic_symbol_id"] = CleanSymbol(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
            ["parser_backend"] = "roslyn_semantic_model"
        };
    }

    private static List<MetadataReference> MetadataReferences()
    {
        var trusted = Convert.ToString(AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"));
        if (string.IsNullOrWhiteSpace(trusted))
        {
            return new List<MetadataReference>();
        }
        return trusted.Split(Path.PathSeparator)
            .Where(File.Exists)
            .Select(path => MetadataReference.CreateFromFile(path))
            .Cast<MetadataReference>()
            .ToList();
    }

    private static IMethodSymbol? ResolveMethod(SemanticModel model, InvocationExpressionSyntax invocation)
    {
        var info = model.GetSymbolInfo(invocation);
        return info.Symbol as IMethodSymbol ?? info.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
    }

    private static string ContainingMethodName(SemanticModel model, SyntaxNode node)
    {
        var method = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (method != null)
        {
            return model.GetDeclaredSymbol(method)?.Name ?? method.Identifier.Text;
        }
        var ctor = node.Ancestors().OfType<ConstructorDeclarationSyntax>().FirstOrDefault();
        if (ctor != null)
        {
            return model.GetDeclaredSymbol(ctor)?.Name ?? ctor.Identifier.Text;
        }
        return "top-level-statements";
    }

    private static string InvokedMethodName(ExpressionSyntax expression)
    {
        if (expression is MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.Name switch
            {
                GenericNameSyntax generic => generic.Identifier.Text,
                IdentifierNameSyntax identifier => identifier.Identifier.Text,
                _ => memberAccess.Name.ToString()
            };
        }
        if (expression is GenericNameSyntax genericName)
        {
            return genericName.Identifier.Text;
        }
        if (expression is IdentifierNameSyntax identifierName)
        {
            return identifierName.Identifier.Text;
        }
        return expression.ToString();
    }

    private static bool IsDiRegistration(string methodName)
    {
        return methodName is "AddScoped" or "AddTransient" or "AddSingleton" or "AddDbContext" or "AddHostedService";
    }

    private static Dictionary<string, object?>? BuildDiRegistration(SemanticModel model, InvocationExpressionSyntax invocation, string methodName, string callerComponent, string relPath, int line, SourceFileRecord row)
    {
        var genericName = (invocation.Expression as MemberAccessExpressionSyntax)?.Name as GenericNameSyntax
            ?? invocation.Expression as GenericNameSyntax;
        var typeArgs = genericName?.TypeArgumentList.Arguments ?? default(SeparatedSyntaxList<TypeSyntax>);
        if (typeArgs.Count == 0)
        {
            return null;
        }

        var serviceType = TypeName(model, typeArgs[0]);
        var implementationType = typeArgs.Count > 1 ? TypeName(model, typeArgs[1]) : serviceType;
        var implementationSimple = SimpleName(implementationType);
        var serviceSimple = SimpleName(serviceType);
        return new Dictionary<string, object?>
        {
            ["dependency_id"] = null,
            ["kind"] = "di_registration",
            ["from"] = row.Project,
            ["to"] = implementationSimple,
            ["relationship"] = "registers",
            ["source_file"] = relPath,
            ["line"] = line,
            ["evidence"] = methodName + " semantically registers " + serviceType + " to " + implementationType,
            ["confidence"] = 0.95,
            ["metadata"] = new Dictionary<string, object?>
            {
                ["parser_backend"] = "roslyn_semantic_model",
                ["lifetime"] = methodName,
                ["service"] = serviceSimple,
                ["service_full_name"] = serviceType,
                ["implementation"] = implementationSimple,
                ["implementation_full_name"] = implementationType,
                ["source_component"] = callerComponent,
                ["resolution_quality"] = "roslyn_semantic_type_binding",
                ["project"] = row.Project,
                ["project_path"] = row.ProjectPath
            }
        };
    }

    private static Dictionary<string, object?>? BuildRouteHint(SemanticModel model, InvocationExpressionSyntax invocation, string methodName, string callerComponent, string relPath, int line, SourceFileRecord row)
    {
        if (!methodName.StartsWith("Map", StringComparison.Ordinal) && !methodName.StartsWith("Http", StringComparison.Ordinal))
        {
            return null;
        }
        var firstArg = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
        var constant = firstArg == null ? default(Optional<object?>) : model.GetConstantValue(firstArg);
        var path = constant.HasValue ? Convert.ToString(constant.Value) : null;
        if (string.IsNullOrWhiteSpace(path) && !methodName.Contains("Controller", StringComparison.OrdinalIgnoreCase) && !methodName.Contains("Razor", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        return new Dictionary<string, object?>
        {
            ["kind"] = "roslyn_route_hint",
            ["method"] = methodName,
            ["path_or_name"] = path ?? methodName,
            ["owning_component"] = callerComponent,
            ["source_file"] = relPath,
            ["line"] = line,
            ["project"] = row.Project,
            ["confidence"] = string.IsNullOrWhiteSpace(path) ? 0.72 : 0.9,
            ["evidence"] = "Roslyn semantic invocation hint for " + methodName
        };
    }

    private static string TypeName(SemanticModel model, TypeSyntax syntax)
    {
        var type = model.GetTypeInfo(syntax).Type;
        return type == null ? syntax.ToString() : CleanSymbol(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
    }

    private static string SimpleName(string value)
    {
        var cleaned = CleanSymbol(value);
        var generic = cleaned.IndexOf('<');
        if (generic >= 0)
        {
            cleaned = cleaned.Substring(0, generic);
        }
        var dot = cleaned.LastIndexOf('.');
        return dot >= 0 ? cleaned.Substring(dot + 1) : cleaned;
    }

    private static string CleanSymbol(string value)
    {
        return value.Replace("global::", "").Trim();
    }

    private static int LineFor(SyntaxTree tree, SyntaxNode node)
    {
        return tree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }

    private static string SafeId(string value)
    {
        return new string(value.Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray());
    }

    private static List<Dictionary<string, object?>> DeduplicateDependencies(List<Dictionary<string, object?>> dependencies)
    {
        var seen = new HashSet<string>();
        var result = new List<Dictionary<string, object?>>();
        foreach (var dependency in dependencies)
        {
            var marker = string.Join("|", new[]
            {
                Convert.ToString(dependency.GetValueOrDefault("kind")) ?? "",
                Convert.ToString(dependency.GetValueOrDefault("from")) ?? "",
                Convert.ToString(dependency.GetValueOrDefault("to")) ?? "",
                Convert.ToString(dependency.GetValueOrDefault("source_file")) ?? "",
                Convert.ToString(dependency.GetValueOrDefault("line")) ?? ""
            });
            if (seen.Add(marker))
            {
                result.Add(dependency);
            }
        }
        return result;
    }
}
