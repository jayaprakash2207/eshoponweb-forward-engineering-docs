# BA Pipeline — Developer Guide

How to set up and run the Business Architecture + Data Architecture extraction pipeline from scratch.

---

## What This Pipeline Does

Given any legacy application (GitHub URL, local folder, or .zip), it produces:

- **10 BA documents** — Capability Map, Value Stream, Process Models, Business Rules, Data Model, Stakeholder Map, KPIs, Motivation Model, Operating Model, Business Roadmap
- **13 DA output files** — Schema Catalogue, ERD, Data Dictionary, PII Inventory, Data Flow Map, Hidden Business Rules, Access Control Matrix, and more
- **1 DA Review Summary** — cross-file consistency check and enrichment report
- **30+ AA output files** — System Inventory, Component Registry, Dependency Graph, API Catalogue, Architecture Violations, Risk Register, Enterprise Blueprint, Migration Wave Plan, and more

---

## Prerequisites

| Tool | Version | Why needed |
|---|---|---|
| Python | 3.10 or later | Runs Layer 1 extraction and all runner scripts |
| Claude Code CLI (`claude`) | Latest | Runs the LLM agents (Layer 2, Layer 3, DA Agent 1, DA Agent 2) |
| Git | Any | Required if source is a git URL |
| VS Code (optional) | Any | Recommended for opening the project and reading outputs |

### Check if Claude Code CLI is installed

```bash
claude --version
```

If not installed, follow the Claude Code setup guide at [claude.ai/code](https://claude.ai/code).

---

## Step 1 — Open the BA Folder in Claude Code

Open VS Code, then open the `bussiness-architecture` folder:

**Option A — via VS Code**
1. Open VS Code
2. `File → Open Folder`
3. Select the `bussiness-architecture` folder
4. Open the Claude Code panel (sidebar icon or `Ctrl+Shift+P` → `Claude Code`)

**Option B — via terminal**
```bash
cd path/to/bussiness-architecture
claude
```

You are now in the project root. All commands below run from this location.

---

## Step 2 — Create a Virtual Environment

```bash
python -m venv .venv
```

Activate it:

**Windows (PowerShell)**
```powershell
.venv\Scripts\Activate.ps1
```

**Windows (Command Prompt)**
```cmd
.venv\Scripts\activate.bat
```

**Mac / Linux**
```bash
source .venv/bin/activate
```

You should see `(.venv)` appear at the start of your terminal prompt.

---

## Step 3 — Install Requirements

```bash
pip install -r requirements.txt
```

Expected output:
```
Successfully installed PyYAML-6.x requests-2.x
```

---

## Step 4 — Verify the Setup

```bash
python run_pipeline.py --help
```

Expected output:
```
usage: run_pipeline.py [-h] --source SOURCE [--output OUTPUT]
                       [--token TOKEN] [--app-url APP_URL] [--full-run]
...
```

If this works, the environment is ready.

---

## Step 5 — Run Layer 1 (Code Extraction)

Layer 1 reads the source application, extracts business artifacts, database objects, and config parameters.

**From a GitHub URL:**
```bash
python run_pipeline.py \
  --source https://github.com/dotnet-architecture/eShopOnWeb \
  --output output/eShopOnWeb
```

**From a local folder:**
```bash
python run_pipeline.py \
  --source C:\path\to\your\app \
  --output output/MyApp
```

**From a .zip file:**
```bash
python run_pipeline.py \
  --source C:\path\to\eShopOnWeb-main.zip \
  --output output/eShopOnWeb
```

**For a private GitHub repo (with token):**
```bash
python run_pipeline.py \
  --source https://github.com/your-org/private-repo \
  --output output/MyApp \
  --token ghp_your_token_here
```

Layer 1 output is saved to `output/eShopOnWeb/`:
```
output/eShopOnWeb/
  source_code.json          ← extracted methods, classes, interfaces, enums
  database.json             ← tables, EF entities, stored procedures, triggers
  config.json               ← connection strings, business params, feature flags
  logs.json                 ← log events and process sequences
  extraction_summary.json   ← counts and metadata
```

---

## Step 6 — Run All Remaining Layers (Recommended)

Use `--full-run` to automatically run all layers in the correct order after Layer 1 completes. All agents run **fully sequentially, one at a time** — no concurrent `claude` CLI calls.

**Fail-fast behavior:** if any step fails (e.g. `claude` CLI not found, agent produced no output files, non-zero exit code), the pipeline **stops immediately** — it does not continue to later steps. It prints which step failed, the exact command run, the exit code, and the captured error/output as the reason. Fix the reported issue and re-run; steps already completed (their output files) are left in place.

```bash
python run_pipeline.py \
  --source https://github.com/dotnet-architecture/eShopOnWeb \
  --output output/eShopOnWeb \
  --full-run
```

**Execution order:**

```
Layer 1  (Python, ~1 min)
    │
    ▼
Layer 2  (claude agent, ~2 min)         ── BA analysis
    │
    ▼
Layer 3  (claude agent, ~3 min)         ── BA documents
    │
    ▼
DA Agent 1 (claude agent, ~3 min)       ── DA extraction
    │
    ▼
DA Agent 2 (claude agent, ~2 min)       ── DA review
    │
    ▼
TA Agent 1 (claude agent, ~3 min)       ── Stack Scout: technology inventory
    │
    ▼
TA Agent 2 (claude agent, ~3 min)       ── Deep Analyst: TA assessment
    │
    ▼
AA Pipeline — Python analyzer (~2 min)  ── inventory → parsed → evidence packs (no LLM)
    │
    ▼
AA Pipeline — claude chain, stages 04→07 (~4 calls, sequential)
    ── final architecture → enterprise forward engineering
       → enterprise blueprint/quality review → workflow audit
```

Full run typically completes in **20–30 minutes** depending on codebase size, since the TA agents and the AA pipeline's 4-stage sequential `claude` chain (stages 04–07) all add to the total. The TA and AA tracks require the extracted repo source and are skipped if `--full-run` did not preserve it (this is automatic).

---

## Step 7 — Run Layers Individually (Manual Mode)

If you prefer to run each layer separately (to review output before proceeding), skip `--full-run` and run each runner manually.

### Layer 2 — Business Architecture Analysis

Reads Layer 1 JSON, calls the claude agent, produces `layer2_output.json`.

```bash
python layer2/layer2_runner.py --input output/eShopOnWeb --run
```

### Layer 3 — BA Document Generation

Reads `layer2_output.json`, generates all 10 BA documents.

```bash
python layer3/layer3_runner.py --input output/eShopOnWeb --run
```

### DA Agent 1 — Data Architecture Extraction

Reads Layer 1 DB and config artifacts, attempts live DB connection, produces 13 DA output files.

```bash
python data-architecture/da_agent1_runner.py --input output/eShopOnWeb --run
```

### DA Agent 2 — Data Architecture Review

Reads the 13 DA files, enriches them with change records, produces `review-summary.md`.

```bash
python data-architecture/da_agent2_runner.py --input output/eShopOnWeb --run
```

### TA Agent 1 — Technology Architecture Stack Scout

Scans the extracted source repo for manifests, Dockerfiles, docker-compose/k8s/Terraform manifests, and CI/CD pipeline files. Requires `--repo-root` (the extracted source path from Layer 1). Produces 6 inventory files under `ta-outputs/ta_agent1/`.

```bash
python technology-architecture/ta_agent1_runner.py --repo-root /path/to/source --input output/eShopOnWeb --run
```

### TA Agent 2 — Technology Architecture Deep Analyst

Reads TA Agent 1's 6 inventory files plus a deeper repo scan (including full CI/CD pipeline file reads), and produces 8 final TA documents plus `ta-review-summary.md` under `ta-outputs/`. Also requires `--repo-root`.

```bash
python technology-architecture/ta_agent2_runner.py --repo-root /path/to/source --input output/eShopOnWeb --run
```

### AA Pipeline — Application Architecture Extraction

Two phases against the source code. Requires the extracted source path from Layer 1.

1. **Python analyzer** (deterministic, no LLM) — inventory → source-chunks → parsed/semantic facts → evidence packs → final/enterprise/blueprint/review JSON & Markdown.
2. **Claude stage chain** (stages 04→07, sequential `claude` calls) — reasons over the Python evidence packs to produce the final architecture, enterprise forward-engineering, blueprint/quality-review, and workflow-audit outputs under `aa-outputs/llm-stages/`.

```bash
# Python analyzer + claude stage chain (default with --run)
python application-architecture/aa_runner.py \
  --repo-root /path/to/source \
  --input output/eShopOnWeb \
  --run

# Python analyzer only — no tokens spent
python application-architecture/aa_runner.py \
  --repo-root /path/to/source \
  --input output/eShopOnWeb \
  --run --skip-llm

# Claude stage chain only — reuses existing evidence-packs/
python application-architecture/aa_runner.py \
  --input output/eShopOnWeb \
  --llm-only
```

> **Note:** When using `--full-run`, the pipeline passes the extracted source path automatically and runs both phases. Only use `aa_runner.py` directly when you have the source folder at hand.

> **Tip:** Remove `--run` from any command to generate the task file only (no claude call). You can then open the task file in Claude Code and ask Claude to process it manually.

---

## Step 8 — View the Outputs

All outputs are inside the directory you specified with `--output`.

### BA Documents (10 files)
```
output/eShopOnWeb/ba_documents/
  01_capability_map.md
  02_value_stream.md
  03_process_models.md
  04_business_rules.md
  05_data_model.md
  06_stakeholder_map.md
  07_kpis_metrics.md
  08_motivation_model.md
  09_operating_model.md
  10_business_roadmap.md
```

### DA Outputs (13 files + review)
```
output/eShopOnWeb/da-outputs/
  schema-catalogue.json
  erd.md
  data-source-inventory.json
  data-flow-map.md
  pii-inventory.json
  data-quality-report.md
  migration-complexity.json
  hidden-business-rules.json
  storage-pattern-analysis.md
  redundancy-analysis.json
  data-dictionary.md
  conceptual-data-model.md
  access-control-matrix.md
  review-summary.md            ← added by DA Agent 2
```

### TA Outputs (6 inventory files + 8 final files + review summary)
```
output/eShopOnWeb/ta-outputs/
  ta_agent1/                              ← TA Agent 1 (Stack Scout) inventory
    technology-stack-inventory.md
    component-service-map.md
    data-store-registry.md
    infrastructure-deployment-blueprint.md
    integration-dependency-graph.md
    security-configuration-snapshot.md

  technology-stack-assessment.md          ← TA Agent 2 (Deep Analyst) final outputs
  architecture-pattern-catalog.md
  component-interaction-contract-map.md
  data-architecture-assessment.md
  security-architecture-assessment.md
  nfr-registry.md
  technical-debt-risk-register.md
  operational-architecture-assessment.md
  ta-review-summary.md                    ← Validation Queue + Agent 1 Discrepancy Log
```

### AA Outputs (30+ files — pure Python, no LLM)
```
output/eShopOnWeb/aa-outputs/
  inventory/
    file-inventory.json        ← all source files catalogued
    project-inventory.json     ← projects and deployable units
    language-summary.json      ← language breakdown
  parsed/
    symbol-registry.json       ← all classes, services, controllers
    route-registry.json        ← all HTTP routes and endpoints
    dependency-candidates.json ← dependency edges between components
    source-chunk-index.json    ← chunked source for evidence building
  evidence-packs/
    system-inventory-pack.json
    module-boundary-pack.json
    component-registry-pack.json
    dependency-pack.json
    entry-point-pack.json
    call-flow-pack.json
    layering-pattern-pack.json
    external-boundary-pack.json
    frontend-application-pack.json
  final/
    application-architecture-summary.md     ← human-readable overview
    system-inventory.json
    component-registry.json                 ← all 300+ components
    dependency-graph.json                   ← full graph with edges
    architecture-pattern-report.md          ← detected pattern (e.g. Layered Monolith)
    architecture-violation-register.json    ← layering violations
    application-risk-register.json          ← risks with severity
    enterprise-application-architecture-blueprint.md  ← full blueprint
    enterprise-application-architecture-blueprint.json
    migration-wave-plan.md
    service-boundary-options.md
    quality-review.md
    executive-summary-for-review.md
    diagrams/                               ← Mermaid diagrams
      system-context.mmd
      container-view.mmd
      component-view.mmd
      dependency-view.mmd
      call-flow-view.mmd
  llm-stages/                               ← claude chain outputs (stages 04→07)
    final/
      ... files emitted by stages 04-final-architecture and 05-enterprise-forward-engineering
    workflow/
      ... files emitted by stages 06-quality-review and 07-workflow-audit
```

> The `llm-stages/` outputs are written separately from the Python `final/` outputs so the
> deterministic analyzer results are never overwritten by the claude chain.

### Agent Task Files (for manual inspection)
```
output/eShopOnWeb/
  layer2_agent_task.md         ← what Layer 2 agent receives
  layer3_agent_task.md         ← what Layer 3 agent receives
  da_agent1_task.md            ← what DA Agent 1 receives
  da_agent2_task.md            ← what DA Agent 2 receives
  layer2_output.json           ← structured BA analysis
  da_agent1_output_raw.txt     ← raw DA Agent 1 response
  da_agent2_output_raw.txt     ← raw DA Agent 2 response
  ta_agent1_task.md            ← what TA Agent 1 (Stack Scout) receives
  ta_agent1_output_raw.txt     ← raw TA Agent 1 response
  ta_agent2_task.md            ← what TA Agent 2 (Deep Analyst) receives
  ta_agent2_output_raw.txt     ← raw TA Agent 2 response
  aa_stage_04-final-architecture_task.md             ← what AA stage 04 receives
  aa_stage_04-final-architecture_raw.txt             ← raw AA stage 04 response
  aa_stage_05-enterprise-forward-engineering_task.md
  aa_stage_05-enterprise-forward-engineering_raw.txt
  aa_stage_06-quality-review_task.md
  aa_stage_06-quality-review_raw.txt
  aa_stage_07-workflow-audit_task.md
  aa_stage_07-workflow-audit_raw.txt
```

---

## Troubleshooting

### `claude CLI not found in PATH`

The runner prints this when it cannot find the `claude` command. Fix:
1. Install Claude Code: [claude.ai/code](https://claude.ai/code)
2. Confirm it is on your PATH: `claude --version`
3. Re-run with `--run`

Without the CLI, generate the task file and run manually in Claude Code:
```bash
# Generate task file (no --run)
python layer2/layer2_runner.py --input output/eShopOnWeb

# Then in Claude Code, say:
# "process layer2_agent_task.md and save output to output/eShopOnWeb/layer2_output.json"
```

### `DB connection: CODE-ONLY`

DA Agent 1 could not reach the database. This is normal if the application is not running locally. The agent will analyse code only — all 13 DA files are still produced, with `db_connection: CODE-ONLY` recorded and the exact error documented.

To connect, start the database first (e.g. `docker-compose up -d`) then re-run `da_agent1_runner.py`.

### `Only N/13 DA files found`

DA Agent 1 did not produce all 13 files. Check:
1. `output/eShopOnWeb/da_agent1_output_raw.txt` — see the raw agent response
2. The agent may have hit a timeout — re-run `da_agent1_runner.py --run`
3. If the task file is large (> 200 KB), the codebase may be too large for a single context window — open a Claude Code issue

### `Only N/6 TA Agent 1 files found` or no `===TA_FILE_START===` markers

TA Agent 1 (Stack Scout) or TA Agent 2 (Deep Analyst) did not produce all expected files. Check:
1. `output/eShopOnWeb/ta_agent1_output_raw.txt` (or `ta_agent2_output_raw.txt`) for the raw response
2. `output/eShopOnWeb/ta_agent1_task.md` (or `ta_agent2_task.md`) to confirm the task size is reasonable — very large repos may need a smaller `max_files`/`cap_per_file` in `technology-architecture/ta_agent1_runner.py`'s `collect_repo_context()`
3. Re-run with `--run`: `python technology-architecture/ta_agent1_runner.py --repo-root <source> --input <output_dir> --run`
4. TA Agent 2 requires TA Agent 1's output first — confirm `ta-outputs/ta_agent1/` has at least 3 of the 6 files

### AA claude stage produced no marker files / `0/4 stages produced output`

The AA stage chain (04→07) emits files wrapped in `===AA_FILE_START:.../===AA_FILE_END===` markers.
If a stage fails to follow this format:
1. Check `output/eShopOnWeb/aa_stage_<id>_raw.txt` for the raw response.
2. Check `output/eShopOnWeb/aa_stage_<id>_task.md` to see exactly what was sent.
3. Re-run just that stage with `aa_runner.py --input <output_dir> --llm-only` (it re-runs all of 04→07; earlier stages will overwrite their previous output).
4. If stage 04 fails, confirm `aa-outputs/evidence-packs/` was populated by the Python analyzer first.

### Layer 1 produces 0 business artifacts

Likely causes:
- Language was not detected correctly — check `extraction_summary.json` → `language` field
- File filter excluded all source files — confirm the source path is correct
- The extractor for that language may not be implemented — check `layer1/extractors/`

---

## Full Command Reference

```bash
# Layer 1 only (always runs Python extraction)
python run_pipeline.py --source <url_or_path> --output <output_dir>

# Layer 1 + all downstream layers (parallel)
python run_pipeline.py --source <url_or_path> --output <output_dir> --full-run

# Layer 2 only (BA analysis)
python layer2/layer2_runner.py --input <output_dir> [--run]

# Layer 3 only (BA document generation)
python layer3/layer3_runner.py --input <output_dir> [--run]

# DA Agent 1 only (data architecture extraction)
python data-architecture/da_agent1_runner.py --input <output_dir> [--run]

# DA Agent 2 only (data architecture review)
python data-architecture/da_agent2_runner.py --input <output_dir> [--run]

# TA Agent 1 only (technology architecture stack scout)
python technology-architecture/ta_agent1_runner.py --repo-root <source_path> --input <output_dir> [--run]

# TA Agent 2 only (technology architecture deep analysis)
python technology-architecture/ta_agent2_runner.py --repo-root <source_path> --input <output_dir> [--run]

# AA Pipeline — Python analyzer + claude stage chain (04-07)
python application-architecture/aa_runner.py --repo-root <source_path> --input <output_dir> --run

# AA Pipeline — Python analyzer only, no claude calls
python application-architecture/aa_runner.py --repo-root <source_path> --input <output_dir> --run --skip-llm

# AA Pipeline — claude stage chain only (reuses existing evidence-packs/)
python application-architecture/aa_runner.py --input <output_dir> --llm-only
```

---

## Project Structure Reference

```
bussiness-architecture/
  run_pipeline.py                        ← main entry point
  requirements.txt

  layer1/                                ← Python extraction (no LLM)
    pipeline.py
    extractors/
      dotnet_extractor.py
      java_extractor.py
      python_extractor.py
      javascript_extractor.py
      base_extractor.py
    database_extractor.py
    config_extractor.py
    log_extractor.py
    file_filter.py
    language_detector.py
    cleaner.py
    output_saver.py
    input_resolver.py

  layer2/                                ← BA analysis agent
    layer2_runner.py
    layer2_prompt.md

  layer3/                                ← BA document generation agent
    layer3_runner.py
    layer3_prompt.md

  data-architecture/                     ← DA extraction and review agents
    da_agent1_runner.py
    da_agent2_runner.py
    DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md
    DA_REVIEW_PROMPT.md

  technology-architecture/               ← TA extraction and analysis agents
    ta_agent1_runner.py                  ← Stack Scout (6 inventory files)
    ta_agent2_runner.py                  ← Deep Analyst (8 final files + review)
    TA_STACKSCOUT_PROMPT.md
    TA_DEEPANALYST_PROMPT.md

  application-architecture/              ← AA extraction pipeline (pure Python)
    aa_runner.py                         ← pipeline wrapper / entry point
    AGENTS.md                            ← stage orchestration rules
    application_architecture_extraction_agent_prompt.md
    architecture-prompts/                ← 8 stage prompts (00–07)
    tools/application_architecture_analyzer/
      run_architecture_extraction.py     ← AA orchestrator (10 stages)
      scan_inventory.py
      generate_source_chunks.py
      extract_parsed_facts.py
      extract_roslyn_semantic_facts.py
      generate_evidence_packs.py
      generate_final_architecture.py
      generate_enterprise_forward_engineering.py
      generate_enterprise_application_architecture_blueprint.py
      generate_review_artifacts.py

  output/                                ← all generated outputs (git-ignored)
    eShopOnWeb/
      ba_documents/                      ← 10 BA markdown files
      da-outputs/                        ← 13 DA files + review-summary.md
      ta-outputs/                        ← 6 TA Agent 1 inventory files + 8 TA Agent 2 files + review summary
      aa-outputs/                        ← 30+ AA files across inventory/, parsed/, evidence-packs/, final/
```
