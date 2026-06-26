# Business Architecture Pipeline - Execution Plan
## Extracting 10 BA Documents from Legacy Applications

**Document Version:** 1.0  
**Date:** May 27, 2026  
**Purpose:** Extract Business Architecture outputs from legacy applications using a 3-layer pipeline

---

## Executive Summary     

This document outlines how to extract 10 Business Architecture (BA) documents from a legacy application using an automated 3-layer pipeline:

1. **Layer 1 (Extraction):** Pull raw artifacts from legacy app using specialized parsers
2. **Layer 2 (Processing):** Use SLM + LLM to transform raw data into structured business insights
3. **Layer 3 (Output):** Generate 10 BA documents ready for upgrade planning

**Expected Outcome:** Complete understanding of legacy application business logic, data, processes, and rules — enabling informed upgrade/migration decisions.

---

## Table of Contents

1. [What is BA and Why It Matters](#what-is-ba-and-why-it-matters)
2. [The 3-Layer Pipeline Architecture](#the-3-layer-pipeline-architecture)
3. [Layer 1: Extraction - Detailed Implementation](#layer-1-extraction--detailed-implementation)
4. [Layer 2: Processing - Detailed Implementation](#layer-2-processing--detailed-implementation)
5. [Layer 3: Output - BA Documents Generated](#layer-3-output--ba-documents-generated)
6. [Implementation Timeline](#implementation-timeline)
7. [Technical Stack & Tools](#technical-stack--tools)
8. [Resource Requirements](#resource-requirements)
9. [Success Metrics & Validation](#success-metrics--validation)
10. [Risk Mitigation](#risk-mitigation)

---

## What is BA and Why It Matters

### Business Architecture Definition

Business Architecture is a strategic framework that documents:

- **What** the business does (Capabilities)
- **How** the business works (Processes)
- **Why** business needs exist (Motivation)
- **Who** is involved (Stakeholders)
- **What data** is used (Information model)
- **What rules** govern operations (Business rules)

### Why Extract BA Before Upgrade?

Legacy upgrade projects fail when:
- ❌ Hidden business rules are missed → System breaks silently
- ❌ Processes are replicated as-is → Broken processes migrate to new system
- ❌ Data dependencies are unknown → Data migration fails
- ❌ Stakeholders are not aligned → Adoption resistance

BA analysis prevents these failures by documenting everything before you build.

### What This Pipeline Delivers

**5 Fully Extracted from Legacy App:**
- Capability Map
- Value Stream Map
- Business Process Models
- Business Rules Inventory
- Information / Data Model

**2 Partially Extracted (need minor validation):**
- Stakeholder Map
- KPIs & Metrics

**3 Inferred (need business confirmation):**
- Business Motivation Model
- Operating Model
- Business Roadmap

---

## The 3-Layer Pipeline Architecture

```
LEGACY APPLICATION
    ├── Source Code
    ├── Database (DDL, Procs, Triggers)
    ├── UI Screens & Menus
    ├── Config Files
    ├── Transaction Logs
    └── Reports
           │
           ▼
    ┌──────────────────────────────────────────┐
    │    LAYER 1: EXTRACTION                   │
    │                                          │
    │  Parsers extract raw artifacts           │
    │  → Store in vector database              │
    │  → Tag by source & type                  │
    └──────────────────────────────────────────┘
           │
           ▼
    ┌──────────────────────────────────────────┐
    │    LAYER 2: PROCESSING                   │
    │                                          │
    │  SLM (Fast, Structured)                  │
    │  └─ Extract entities, rules, sequences   │
    │     → Output: Clean JSON structures      │
    │                                          │
    │  LLM (Deep Reasoning)                    │
    │  └─ Interpret, contextualize, generate   │
    │     → Output: BA narratives & diagrams   │
    └──────────────────────────────────────────┘
           │
           ▼
    ┌──────────────────────────────────────────┐
    │    LAYER 3: OUTPUT                       │
    │                                          │
    │  10 Business Architecture Documents      │
    │  ├─ Capability Map                       │
    │  ├─ Value Stream Map                     │
    │  ├─ Business Process Models              │
    │  ├─ Business Rules Inventory             │
    │  ├─ Information / Data Model             │
    │  ├─ Stakeholder Map                      │
    │  ├─ KPIs & Metrics                       │
    │  ├─ Business Motivation Model (inferred) │
    │  ├─ Operating Model (inferred)           │
    │  └─ Business Roadmap (inferred)          │
    └──────────────────────────────────────────┘
```

---

## Layer 1: Extraction - Detailed Implementation

### Purpose
Extract all raw artifacts from legacy application and store them in a searchable, structured format.

### Step 1.1: Source Code Extraction

**What to Extract:**
- Business logic methods
- Validation functions
- Constants & enums
- Class & package structure
- Comments & annotations
- Integration code

**Tools by Language:**

| Language | Parser Tool | Command |
|----------|------------|---------|
| Java | JavaParser | `mvn dependency:copy-dependencies` + custom script |
| C# / .NET | Roslyn | `dotnet` CLI + C# analyzer |
| Python | AST module | `python ast_parser.py` |
| COBOL | OpenCOBOL Parser | Command-line extraction |
| VB6 | VBParser | Custom parsing tool |
| JavaScript | Babel / Acorn | Node.js based parser |

**Implementation:**

```python
# Example: Java Code Extraction (pseudocode)
import subprocess
import json

def extract_java_methods(source_dir):
    """Extract business logic from Java source"""
    
    # 1. Parse AST using JavaParser
    result = subprocess.run([
        'javaparser', 
        '--output-format', 'json',
        source_dir
    ], capture_output=True)
    
    ast_data = json.loads(result.stdout)
    
    # 2. Filter for business logic
    business_methods = []
    for class_def in ast_data['classes']:
        for method in class_def['methods']:
            if is_business_method(method):
                business_methods.append({
                    'class': class_def['name'],
                    'method': method['name'],
                    'body': method['body'],
                    'parameters': method['parameters'],
                    'source_file': class_def['file']
                })
    
    # 3. Store as JSON chunks
    return business_methods

def is_business_method(method):
    """Heuristic: methods that validate, calculate, or decide"""
    keywords = ['validate', 'calculate', 'apply', 'check', 'process', 
                'create', 'update', 'approve', 'reject']
    return any(kw in method['name'].lower() for kw in keywords)
```

**Output:** JSON file with methods tagged by business category

---

### Step 1.2: Database Extraction

**What to Extract:**
- Table DDL (CREATE TABLE statements)
- Stored procedures
- Triggers
- Views
- Foreign key relationships
- Reference tables (lookups)

**Tools:**

| Database | Tool | Command |
|----------|------|---------|
| Oracle | SQL Developer | Export DDL via UI |
| SQL Server | SQL Server Management Studio | Script all objects |
| PostgreSQL | pg_dump | `pg_dump --schema-only` |
| MySQL | mysqldump | `mysqldump --no-data` |

**Implementation:**

```bash
#!/bin/bash
# Extract database schema from SQL Server

SQLSERVER_HOST="legacy-db.company.com"
DB_NAME="BookStore"
OUTPUT_FILE="db_schema.sql"

# 1. Export all DDL
sqlcmd -S $SQLSERVER_HOST -d $DB_NAME -Q "
    SELECT OBJECT_DEFINITION(OBJECT_ID(table_name))
    FROM information_schema.tables
" > $OUTPUT_FILE

# 2. Export stored procedures
sqlcmd -S $SQLSERVER_HOST -d $DB_NAME -Q "
    SELECT ROUTINE_DEFINITION
    FROM information_schema.routines
    WHERE ROUTINE_TYPE = 'PROCEDURE'
" >> $OUTPUT_FILE

# 3. Export triggers
sqlcmd -S $SQLSERVER_HOST -d $DB_NAME -Q "
    SELECT definition FROM sys.sql_modules
    WHERE object_id IN (SELECT object_id FROM sys.triggers)
" >> $OUTPUT_FILE

echo "Database schema exported to $OUTPUT_FILE"
```

**Output:** Complete DDL with all objects documented

---

### Step 1.3: UI Screen Extraction

**What to Extract:**
- Screen names & titles
- Form fields & labels
- Menu structure
- Dropdown values
- Validation messages
- Navigation paths

**Tools:**

| Platform | Tool | Method |
|----------|------|--------|
| Web App | Selenium + BeautifulSoup | Automate browser + parse HTML |
| Desktop | WinAppDriver | UI Automation framework |
| JSP/HTML | Scrapy | Web scraping |
| PDF Forms | PyMuPDF | Extract form fields |

**Implementation:**

```python
# UI Screen Extraction from Web App
from selenium import webdriver
from bs4 import BeautifulSoup
import json

def extract_ui_screens():
    """Crawl web application and extract screen structure"""
    
    driver = webdriver.Chrome()
    visited_urls = set()
    screens = []
    
    def crawl_page(url):
        if url in visited_urls:
            return
        visited_urls.add(url)
        
        driver.get(url)
        soup = BeautifulSoup(driver.page_source, 'html.parser')
        
        # Extract screen info
        screen = {
            'url': url,
            'title': soup.find('title').text if soup.find('title') else 'Untitled',
            'menu_path': extract_menu_path(driver),
            'form_fields': extract_form_fields(soup),
            'buttons': extract_buttons(soup),
            'dropdowns': extract_dropdown_options(soup),
            'validation_messages': extract_validation_messages(soup)
        }
        screens.append(screen)
        
        # Follow links
        for link in soup.find_all('a', href=True):
            next_url = link['href']
            if is_internal_link(next_url) and next_url not in visited_urls:
                crawl_page(next_url)
    
    # Start crawl
    crawl_page('http://legacy-app.internal/login')
    
    # Save results
    with open('ui_screens.json', 'w') as f:
        json.dump(screens, f, indent=2)
    
    driver.quit()
    return screens

def extract_form_fields(soup):
    """Extract all form fields with labels"""
    fields = []
    for input_elem in soup.find_all(['input', 'textarea', 'select']):
        label = soup.find('label', {'for': input_elem.get('id')})
        fields.append({
            'name': input_elem.get('name'),
            'type': input_elem.get('type'),
            'label': label.text if label else input_elem.get('placeholder'),
            'required': input_elem.get('required') is not None
        })
    return fields

def extract_dropdown_options(soup):
    """Extract dropdown values (often = business reference data)"""
    dropdowns = {}
    for select in soup.find_all('select'):
        options = [opt.text for opt in select.find_all('option')]
        dropdowns[select.get('name')] = options
    return dropdowns
```

**Output:** Structured JSON with all screens, fields, and navigation paths

---

### Step 1.4: Configuration File Extraction

**What to Extract:**
- Business parameters (thresholds, limits)
- Role definitions
- Feature flags
- Integration endpoints
- Environment settings

**Implementation:**

```python
# Parse config files (XML, YAML, Properties, JSON)
import xml.etree.ElementTree as ET
import yaml
import json
from configparser import ConfigParser

def extract_all_configs(config_dir):
    """Extract from all config file types"""
    
    all_configs = {}
    
    # 1. Parse properties files
    for prop_file in glob.glob(f'{config_dir}/*.properties'):
        config = ConfigParser()
        config.read(prop_file)
        all_configs[prop_file] = dict(config.items())
    
    # 2. Parse YAML files
    for yaml_file in glob.glob(f'{config_dir}/*.yml'):
        with open(yaml_file) as f:
            all_configs[yaml_file] = yaml.safe_load(f)
    
    # 3. Parse XML files
    for xml_file in glob.glob(f'{config_dir}/*.xml'):
        tree = ET.parse(xml_file)
        root = tree.getroot()
        all_configs[xml_file] = et_to_dict(root)
    
    # 4. Parse JSON files
    for json_file in glob.glob(f'{config_dir}/*.json'):
        with open(json_file) as f:
            all_configs[json_file] = json.load(f)
    
    # 5. Identify business rules from config
    business_rules = extract_business_params(all_configs)
    
    return all_configs, business_rules

def extract_business_params(configs):
    """Find business-relevant parameters"""
    keywords = ['max', 'min', 'limit', 'threshold', 'rate', 'percent',
                'approval', 'discount', 'tax', 'fee', 'timeout']
    
    business_params = {}
    for file, config in configs.items():
        for key, value in config.items():
            if any(kw in key.lower() for kw in keywords):
                business_params[key] = value
    
    return business_params
```

**Output:** Extracted config parameters tagged as business rules or settings

---

### Step 1.5: Log & Report Extraction

**What to Extract:**
- Process sequences from transaction logs
- Business metrics from reports
- User activity patterns
- Peak load patterns

**Implementation:**

```python
# Extract process sequences from logs
import re
from collections import defaultdict

def extract_process_sequences(log_file):
    """Mine common process flows from logs"""
    
    sessions = defaultdict(list)
    
    with open(log_file) as f:
        for line in f:
            # Parse: TIMESTAMP | USER | ACTION | OBJECT | RESULT
            match = re.match(
                r'(\d{4}-\d{2}-\d{2}) \| (\w+) \| (\w+) \| ([^\|]*) \| (\w+)',
                line
            )
            if not match:
                continue
            
            timestamp, user, action, obj, result = match.groups()
            sessions[user].append({
                'time': timestamp,
                'action': action,
                'object': obj,
                'result': result
            })
    
    # Find common sequences
    sequences = defaultdict(int)
    for user, events in sessions.items():
        for i in range(len(events) - 2):
            seq = ' → '.join([
                events[i]['action'],
                events[i+1]['action'],
                events[i+2]['action']
            ])
            sequences[seq] += 1
    
    # Return top sequences (common business processes)
    return sorted(sequences.items(), key=lambda x: x[1], reverse=True)[:20]

def extract_report_metrics(report_file):
    """Extract business metrics from reports"""
    
    if report_file.endswith('.pdf'):
        metrics = extract_from_pdf(report_file)
    elif report_file.endswith('.xlsx'):
        metrics = extract_from_excel(report_file)
    else:
        metrics = extract_from_csv(report_file)
    
    return metrics
```

**Output:** Discovered process flows and business metrics

---

### Step 1.6: Store in Vector Database

**What to Do:**
- Clean and normalize all extracted data
- Embed text chunks using embeddings model
- Store in vector database for semantic search

**Implementation:**

```python
# Store extracted artifacts in vector database
from langchain.embeddings import OpenAIEmbeddings
from langchain.vectorstores import Chroma
from langchain.document_loaders import JSONLoader

def store_in_vector_db(extraction_outputs):
    """
    Takes all extracted artifacts and stores them in vector DB
    for retrieval during processing phase
    """
    
    # 1. Combine all extracted data
    documents = []
    
    # Add source code chunks
    for method in extraction_outputs['source_code']:
        documents.append({
            'content': f"Method: {method['class']}.{method['method']}\n{method['body']}",
            'source': 'source_code',
            'type': 'method',
            'metadata': method
        })
    
    # Add database objects
    for table in extraction_outputs['database']['tables']:
        documents.append({
            'content': f"Table: {table['name']}\nColumns: {table['columns']}",
            'source': 'database',
            'type': 'table',
            'metadata': table
        })
    
    # Add UI screens
    for screen in extraction_outputs['ui_screens']:
        documents.append({
            'content': f"Screen: {screen['title']}\nFields: {screen['form_fields']}",
            'source': 'ui',
            'type': 'screen',
            'metadata': screen
        })
    
    # Add config parameters
    for param, value in extraction_outputs['config'].items():
        documents.append({
            'content': f"Parameter: {param} = {value}",
            'source': 'config',
            'type': 'parameter',
            'metadata': {'param': param, 'value': value}
        })
    
    # 2. Generate embeddings
    embeddings = OpenAIEmbeddings()
    
    # 3. Store in ChromaDB
    vector_store = Chroma.from_documents(
        documents,
        embeddings,
        collection_name="legacy_app_artifacts"
    )
    
    print(f"✅ Stored {len(documents)} artifacts in vector database")
    return vector_store
```

**Output:** Searchable vector database with all legacy app artifacts

---

## Layer 2: Processing - Detailed Implementation

### Purpose
Transform raw extracted data into structured business insights using SLM + LLM.

### Step 2.1: SLM Processing (Fast, Structured)

**What SLM Does:**
- Classifies chunks by business category
- Tags rule candidates
- Extracts entities
- Sequences process steps
- Profiles user roles

**Models to Use:**
- Phi-3 Mini (lightweight code understanding)
- Mistral 7B (rule extraction)
- CodeLlama 7B (source code analysis)
- TinyLlama (quick classification)

**Implementation:**

```python
# SLM Processing Pipeline
from transformers import pipeline
import json

class SLMProcessor:
    """Small Language Model for structured extraction"""
    
    def __init__(self):
        # Load local SLM
        self.slm = pipeline(
            "text-generation",
            model="mistralai/Mistral-7B",
            device=0  # GPU
        )
    
    def extract_business_rules(self, code_snippets):
        """Extract rules from code using SLM"""
        
        rules = []
        
        for snippet in code_snippets:
            prompt = f"""Extract business rules from this code.
Return JSON with: rule_id, type, entity, condition, action, priority.

Code:
{snippet['body']}

JSON Output:"""
            
            response = self.slm(prompt, max_length=500)
            
            try:
                rule_json = json.loads(response[0]['generated_text'])
                rule_json['source'] = snippet['source_file']
                rules.append(rule_json)
            except json.JSONDecodeError:
                print(f"Failed to parse rule from {snippet['class']}")
        
        return rules
    
    def extract_entities(self, database_schema):
        """Extract business entities from DB schema"""
        
        entities = []
        
        for table in database_schema['tables']:
            prompt = f"""Analyze this database table and classify it.
Return JSON with: entity_name, business_type, entity_role, attributes, criticality.

Table Definition:
{table['ddl']}

JSON Output:"""
            
            response = self.slm(prompt, max_length=400)
            
            try:
                entity_json = json.loads(response[0]['generated_text'])
                entity_json['table_name'] = table['name']
                entities.append(entity_json)
            except json.JSONDecodeError:
                pass
        
        return entities
    
    def sequence_processes(self, logs):
        """Find process sequences from logs"""
        
        prompt = f"""Identify the main business process sequences from these logs.
Return JSON array with: process_name, steps[], actors[].

Logs:
{logs}

JSON Output:"""
        
        response = self.slm(prompt, max_length=600)
        
        try:
            processes = json.loads(response[0]['generated_text'])
            return processes
        except:
            return []
    
    def profile_roles(self, access_logs, role_configs):
        """Create role profiles from system data"""
        
        prompt = f"""Based on access logs and role configs, create user role profiles.
Return JSON with: role_name, responsibilities[], access_level, system_usage.

Role Configs:
{role_configs}

Access Patterns:
{access_logs}

JSON Output:"""
        
        response = self.slm(prompt, max_length=500)
        
        try:
            roles = json.loads(response[0]['generated_text'])
            return roles
        except:
            return []

# Usage
slm = SLMProcessor()

# Process extracted data
rules = slm.extract_business_rules(extracted_code)
entities = slm.extract_entities(extracted_database)
processes = slm.sequence_processes(extracted_logs)
roles = slm.profile_roles(access_logs, role_configs)

print(f"✅ SLM extracted {len(rules)} rules, {len(entities)} entities, {len(processes)} processes")

# Save SLM outputs
with open('slm_outputs.json', 'w') as f:
    json.dump({
        'rules': rules,
        'entities': entities,
        'processes': processes,
        'roles': roles
    }, f, indent=2)
```

**Output:** Structured JSON with extracted rules, entities, processes, roles

---

### Step 2.2: LLM Processing (Deep Reasoning & Generation)

**What LLM Does:**
- Interprets business meaning
- Generates capability names
- Writes process narratives
- Produces final BA documents

**Models to Use:**
- Claude Sonnet 4
- GPT-4
- Llama 3 70B

**Implementation:**

```python
# LLM Processing Pipeline
from anthropic import Anthropic

class LLMProcessor:
    """Large Language Model for business reasoning & document generation"""
    
    def __init__(self):
        self.client = Anthropic()
    
    def generate_capability_map(self, slm_entities, slm_rules, ui_menus):
        """Generate business capability map from SLM outputs"""
        
        prompt = f"""You are a business architect. Convert these technical findings 
into a 3-level business capability map.

Technical Entities (from database):
{json.dumps(slm_entities, indent=2)}

Business Rules Found:
{json.dumps(slm_rules[:10], indent=2)}  # Top 10 rules

UI Menu Structure:
{json.dumps(ui_menus, indent=2)}

Create a JSON output with structure:
{{
  "L1": [
    {{
      "name": "capability name",
      "description": "what business does",
      "L2": [
        {{
          "name": "sub-capability",
          "description": "...",
          "L3": [
            {{"name": "...", "description": "...", "status": "active/dormant"}}
          ]
        }}
      ]
    }}
  ]
}}

Output ONLY valid JSON, no markdown."""
        
        message = self.client.messages.create(
            model="claude-sonnet-4-20250514",
            max_tokens=2000,
            messages=[{"role": "user", "content": prompt}]
        )
        
        try:
            cap_map = json.loads(message.content[0].text)
            return cap_map
        except:
            return None
    
    def generate_value_stream(self, slm_processes, process_logs):
        """Generate value stream map"""
        
        prompt = f"""As a business architect, convert these technical process 
sequences into a business value stream map.

Processes Identified:
{json.dumps(slm_processes, indent=2)}

Transaction Log Sequences:
{process_logs}

Create a JSON output representing the value stream with stages, actors, handoffs:
{{
  "name": "stream name",
  "stages": [
    {{
      "stage": "name",
      "description": "what happens",
      "actor": "who does it",
      "value_add": true/false,
      "handoff_to": "next stage"
    }}
  ]
}}

Output ONLY valid JSON."""
        
        message = self.client.messages.create(
            model="claude-sonnet-4-20250514",
            max_tokens=2000,
            messages=[{"role": "user", "content": prompt}]
        )
        
        try:
            vs_map = json.loads(message.content[0].text)
            return vs_map
        except:
            return None
    
    def generate_process_models(self, slm_processes, slm_rules):
        """Generate BPMN process models"""
        
        prompt = f"""Convert these process sequences into formal BPMN descriptions.

Processes:
{json.dumps(slm_processes, indent=2)}

Related Business Rules:
{json.dumps(slm_rules, indent=2)}

Create formal process descriptions in JSON:
{{
  "process_name": "...",
  "trigger": "what starts it",
  "actors": ["role1", "role2"],
  "steps": [
    {{
      "step": 1,
      "action": "description",
      "actor": "who does it",
      "decision": false,
      "next_step": 2
    }}
  ],
  "decision_points": [
    {{
      "step": X,
      "condition": "description",
      "then_step": Y,
      "else_step": Z
    }}
  ]
}}

Output ONLY valid JSON."""
        
        message = self.client.messages.create(
            model="claude-sonnet-4-20250514",
            max_tokens=2500,
            messages=[{"role": "user", "content": prompt}]
        )
        
        try:
            processes = json.loads(message.content[0].text)
            return processes
        except:
            return None
    
    def generate_business_rules_doc(self, slm_rules):
        """Generate business rules inventory document"""
        
        prompt = f"""Convert these technical rules into business-readable rules.

Raw Rules:
{json.dumps(slm_rules, indent=2)}

For each rule, create a business definition with:
- Rule ID
- Business name
- Business statement (what it does in business terms)
- Category (validation, calculation, approval, etc)
- Priority (critical, high, medium, low)
- Regulatory relevance if any

JSON format:
{{
  "rules": [
    {{
      "rule_id": "BR001",
      "business_name": "...",
      "business_statement": "...",
      "category": "...",
      "priority": "...",
      "source_location": "...",
      "regulatory": "..."
    }}
  ]
}}

Output ONLY valid JSON."""
        
        message = self.client.messages.create(
            model="claude-sonnet-4-20250514",
            max_tokens=3000,
            messages=[{"role": "user", "content": prompt}]
        )
        
        try:
            rules_doc = json.loads(message.content[0].text)
            return rules_doc
        except:
            return None
    
    def generate_all_ba_outputs(self, slm_outputs, extraction_data):
        """Generate all 10 BA documents"""
        
        results = {}
        
        # 1. Capability Map
        print("📊 Generating Capability Map...")
        results['capability_map'] = self.generate_capability_map(
            slm_outputs['entities'],
            slm_outputs['rules'],
            extraction_data['ui_menus']
        )
        
        # 2. Value Stream Map
        print("🌊 Generating Value Stream Map...")
        results['value_stream'] = self.generate_value_stream(
            slm_outputs['processes'],
            extraction_data['process_logs']
        )
        
        # 3. Process Models
        print("🔄 Generating Process Models...")
        results['process_models'] = self.generate_process_models(
            slm_outputs['processes'],
            slm_outputs['rules']
        )
        
        # 4. Business Rules
        print("📋 Generating Business Rules Inventory...")
        results['business_rules'] = self.generate_business_rules_doc(
            slm_outputs['rules']
        )
        
        # 5. Data Model
        print("🗄️  Generating Information / Data Model...")
        results['data_model'] = self.generate_data_model(
            slm_outputs['entities'],
            extraction_data['database_relationships']
        )
        
        # 6. Stakeholder Map
        print("👥 Generating Stakeholder Map...")
        results['stakeholder_map'] = self.generate_stakeholder_map(
            slm_outputs['roles'],
            extraction_data['org_structure']
        )
        
        # 7. KPIs & Metrics
        print("📈 Generating KPIs & Metrics...")
        results['kpis'] = self.generate_kpis(
            extraction_data['reports'],
            extraction_data['metrics']
        )
        
        # 8-10. Inferred documents (need business input)
        print("⚠️  Inferred outputs (need business validation)...")
        results['motivation_model'] = "INFERRED - Needs business strategy input"
        results['operating_model'] = "INFERRED - Needs org chart input"
        results['roadmap'] = "INFERRED - Needs business direction input"
        
        return results

# Usage
llm = LLMProcessor()

print("\n" + "="*60)
print("LAYER 2: PROCESSING WITH SLM + LLM")
print("="*60 + "\n")

ba_outputs = llm.generate_all_ba_outputs(slm_outputs, extraction_data)

print("\n✅ All BA documents generated!")

# Save LLM outputs
with open('ba_outputs.json', 'w') as f:
    json.dump(ba_outputs, f, indent=2)
```

**Output:** 10 BA documents in structured JSON format

---

## Layer 3: Output - BA Documents Generated

### Output 1: Capability Map

```json
{
  "name": "BookStore Business Capability Map",
  "description": "3-level breakdown of business capabilities",
  "L1_capabilities": [
    {
      "id": "C001",
      "name": "Customer Management",
      "description": "Acquire, onboard, and maintain customer relationships",
      "status": "active",
      "L2": [
        {
          "id": "C001.1",
          "name": "Customer Acquisition",
          "L3": [
            {
              "id": "C001.1.1",
              "name": "Customer Registration",
              "status": "active"
            }
          ]
        }
      ]
    }
  ]
}
```

### Output 2: Value Stream Map

```json
{
  "name": "Book Purchase to Delivery",
  "stages": [
    {
      "stage": 1,
      "name": "Browse & Search",
      "actor": "Customer",
      "description": "Customer searches for books",
      "value_add": true
    },
    {
      "stage": 2,
      "name": "Order Creation",
      "actor": "System",
      "description": "Customer places order",
      "value_add": true
    }
  ]
}
```

### Output 3: Business Process Models

```json
{
  "process_name": "Create Book Order",
  "trigger": "Customer clicks 'Buy Now'",
  "actors": ["Customer", "System", "Manager"],
  "steps": [
    {
      "step": 1,
      "action": "Customer enters order details",
      "actor": "Customer",
      "decision": false
    }
  ]
}
```

### Output 4: Business Rules Inventory

```json
{
  "rules": [
    {
      "rule_id": "BR001",
      "business_name": "Blocked Customer Restriction",
      "business_statement": "A customer with BLOCKED status cannot place orders",
      "category": "Validation",
      "priority": "Critical",
      "source_location": "OrderService.java:12"
    }
  ]
}
```

### Output 5: Information / Data Model

```json
{
  "entities": [
    {
      "entity_name": "Customer",
      "business_definition": "A person registered to purchase books",
      "attributes": ["ID", "Name", "Email", "Status"],
      "relationships": ["places Orders"]
    }
  ]
}
```

### Outputs 6-7: Partial Outputs

- **Stakeholder Map:** Extracted from system roles, needs business input for influence levels
- **KPIs & Metrics:** Extracted from reports, needs business input for targets

### Outputs 8-10: Inferred (Need Business Validation)

- **Business Motivation Model:** Vision, mission, goals, drivers
- **Operating Model:** Organizational structure, decision authority
- **Business Roadmap:** Future capability plans, investment priorities

---

## Implementation Timeline

### Phase 1: Preparation (Week 1)

| Day | Task | Owner | Output |
|-----|------|-------|--------|
| 1-2 | Inventory legacy application (code, DB, configs) | Tech Lead | Technology assessment |
| 3 | Set up parser tools for target language | Developer | Parser tools configured |
| 4-5 | Set up vector database (ChromaDB/Pinecone) | DevOps | DB instance ready |

### Phase 2: Layer 1 Extraction (Weeks 2-3)

| Week | Task | Owner | Deliverable |
|------|------|-------|------------|
| 2 | Extract source code, database, UI | Developer | Raw artifacts |
| 2 | Extract config files, logs, reports | QA/DevOps | Additional artifacts |
| 3 | Normalize, clean, embed all data | Developer | Vector database populated |

### Phase 3: Layer 2 Processing (Week 4)

| Day | Task | Owner | Output |
|-----|------|-------|--------|
| 1-2 | Run SLM on extracted data | ML Engineer | Structured JSON |
| 3-4 | Run LLM to generate BA documents | ML Engineer | 10 BA outputs |
| 5 | Validate and review outputs | BA / Tech Lead | Quality check |

### Phase 4: Layer 3 Output & Validation (Week 5)

| Day | Task | Owner | Output |
|-----|------|-------|--------|
| 1-2 | Format 10 BA documents | BA | Professional documents |
| 3 | Stakeholder validation workshop | BA / Stakeholders | Feedback incorporated |
| 4-5 | Final review and sign-off | Management | Approved BA documents |

**Total Duration:** 5 weeks (one business cycle)

---

## Technical Stack & Tools

### Layer 1: Extraction Tools

```
Language Parsing
├── JavaParser (Java)
├── Roslyn (C#/.NET)
├── AST module (Python)
├── OpenCOBOL (COBOL)
└── Babel/Acorn (JavaScript)

Database Tools
├── SchemaSpy (schema visualization)
├── SQL Parser (statement parsing)
├── DataGrip (SQL querying)
└── pypg (PostgreSQL)

UI Extraction
├── Selenium (web browser automation)
├── Playwright (modern automation)
├── WinAppDriver (Windows desktop)
└── BeautifulSoup (HTML parsing)

Vector Database
├── ChromaDB (local, lightweight)
├── Pinecone (cloud, scalable)
└── Weaviate (self-hosted)

Embeddings
├── OpenAI embeddings
├── Sentence Transformers
└── Ollama (local models)
```

### Layer 2: Processing Models

```
SLM (Small Language Models)
├── Phi-3 Mini (4.7B params, efficient)
├── Mistral 7B (fast, structured tasks)
├── CodeLlama 7B (code understanding)
└── TinyLlama (3B, minimal resources)

LLM (Large Language Models)
├── Claude Sonnet 4 (recommended)
├── GPT-4 (alternative)
├── Llama 3 70B (open source)
└── Mixtral 8×7B (faster alternative)

Infrastructure
├── Docker (containerization)
├── NVIDIA GPU (A100 recommended)
└── Python 3.10+ (runtime)
```

### Layer 3: Output Tools

```
Document Generation
├── Jinja2 (templating)
├── ReportLab (PDF generation)
├── python-docx (Word docs)
└── Pandoc (format conversion)

Visualization
├── Graphviz (diagrams)
├── PlantUML (UML diagrams)
├── Mermaid (flowcharts)
└── D3.js (interactive charts)
```

---

## Resource Requirements

### Hardware

```
GPU Server (recommended):
- GPU: NVIDIA A100 80GB (for LLM inference)
- CPU: 32-core Intel Xeon
- RAM: 256GB
- Storage: 2TB NVMe SSD
- Network: 10Gbps

Budget: $15,000 - $25,000

Alternative (cloud-based):
- AWS: p3.8xlarge instance (~$12/hour)
- Google Cloud: TPU v4 (~$10/hour)
- Azure: A100 instance (~$10/hour)
```

### Personnel

```
Team Composition:
├── 1 Technical Lead (oversee extraction)
├── 1 Backend Developer (implement extractors)
├── 1 ML Engineer (LLM/SLM processing)
├── 1 Business Architect (validate outputs)
├── 0.5 DevOps (database, infrastructure)
└── 0.5 QA (testing & validation)

Total: ~4.5 FTE for 5 weeks
```

### Software Licenses

```
Optional Paid Tools:
├── GitHub Enterprise (code analysis)
├── Pinecone Pro ($1000/month)
├── OpenAI API (usage-based)
└── JetBrains Tools ($1500/year)

Total: ~$2,000-5,000 for 5 weeks
```

---

## Success Metrics & Validation

### Metrics to Track

| Metric | Target | How to Measure |
|--------|--------|----------------|
| **Extraction Completeness** | >95% of code parsed | Lines of code extracted / Total LoC |
| **Rule Discovery** | >100 business rules | Count of BR001, BR002, etc. |
| **Entity Extraction** | >95% of DB objects | Database objects found / Total objects |
| **Process Identification** | >10 major processes | Process models with >5 steps |
| **Document Completeness** | 10/10 BA outputs | All 7 full + 3 inferred documents |
| **Stakeholder Validation** | >80% agreement | Stakeholder survey scores |

### Validation Checklist

**After Layer 1 (Extraction):**
- ✅ All source code files parsed
- ✅ Database schema 100% extracted
- ✅ UI screens captured with fields
- ✅ Configuration parameters identified
- ✅ Business rules candidates identified in code

**After Layer 2 (Processing):**
- ✅ SLM structured outputs are valid JSON
- ✅ Entity extraction matches database objects
- ✅ Process sequences are logical
- ✅ Rule classifications are accurate
- ✅ LLM outputs are readable and accurate

**After Layer 3 (Output):**
- ✅ 10 BA documents are complete
- ✅ Diagrams are generated (BPMN, UML)
- ✅ Business language is used (not technical)
- ✅ Stakeholders recognize their processes
- ✅ Documents are signed off by business

---

## Risk Mitigation

### Risk 1: Legacy Code is Undocumented or Obfuscated

**Risk Level:** HIGH  
**Impact:** Cannot extract business rules

**Mitigation:**
- Conduct interviews with legacy developers
- Use runtime debugging to trace business logic
- Create manual mapping of key processes
- Fall back to log analysis for rule discovery

**Action:** Week 1 - Schedule developer interviews before extraction

---

### Risk 2: Database is Denormalized or Has Poor Naming

**Risk Level:** MEDIUM  
**Impact:** Data model extraction may be incomplete

**Mitigation:**
- Conduct data profiling to understand actual structure
- Interview data owners
- Validate against reports and UI screens
- Manual review of critical tables

**Action:** Week 2 - Data steward reviews database schema output

---

### Risk 3: SLM/LLM Produces Hallucinated Outputs

**Risk Level:** MEDIUM  
**Impact:** Inaccurate BA documents

**Mitigation:**
- Always validate LLM outputs against source data
- Cross-check rules across multiple sources
- Have business SME review all inferred documents
- Use confidence scores and flag uncertain outputs

**Action:** Every output reviewed before stakeholder presentation

---

### Risk 4: Process is Too Slow or Resource-Intensive

**Risk Level:** LOW  
**Impact:** Timeline delays, cost overruns

**Mitigation:**
- Start with pilot on core legacy application module
- Use lighter SLM models for initial extraction
- Parallelize Layer 1 extraction by source type
- Consider cloud-based GPU for Layer 2

**Action:** Week 0 - Start with 10% scope pilot

---

### Risk 5: Stakeholders Don't Recognize Extracted Processes

**Risk Level:** MEDIUM  
**Impact:** Inferred outputs not trusted

**Mitigation:**
- Conduct validation workshops with power users
- Create side-by-side comparison (current process vs extracted process)
- Capture feedback and iterate
- Clearly mark inferred vs extracted outputs

**Action:** Week 4 - Validation workshop scheduled

---

## How This Enables Upgrade Success

Once you have the 10 BA documents, you can:

```
DECISION MAKING:
├─ Know exactly what capabilities to keep/redesign/retire
├─ Prioritize upgrade phases based on value streams
└─ Make data-driven architectural decisions

REQUIREMENTS DEFINITION:
├─ Business requirements from processes and rules
├─ Data requirements from information model
├─ Integration requirements from process flows
└─ Compliance requirements from regulatory rules

RISK MANAGEMENT:
├─ Know all hidden business logic BEFORE upgrade
├─ Identify data migration complexity upfront
├─ Plan for integration testing needs
└─ Identify compliance and regulatory risks early

STAKEHOLDER ALIGNMENT:
├─ Everyone sees their process in the BA documents
├─ Business objectives are explicit
├─ Clear picture of what's changing
└─ Higher adoption rates for new system
```

---

## Next Steps

1. **Review this document** with technical and business leadership
2. **Estimate costs** based on your infrastructure choices
3. **Identify target legacy application** to pilot
4. **Allocate resources** (developers, ML engineer, BA)
5. **Schedule Week 1 kickoff** to start preparation phase
6. **Plan validation workshop** for Week 4 stakeholder review

---

## Appendix A: Example Output - Bookstore Legacy App

### Extracted Capability Map (Partial)

```
L1: Customer Management
  L2: Customer Acquisition
    L3: Customer Registration ✅
    L3: Email Verification ✅
  L2: Customer Relationship
    L3: Purchase History Tracking ✅
    L3: Loyalty Program ✅ (found discount rule)
    
L1: Catalog Management
  L2: Book Inventory
    L3: Add Book ✅
    L3: Update Stock ✅ (from logs)
  L2: Category Management
    L3: Manage Categories ✅

L1: Order Management
  L2: Order Processing
    L3: Create Order ✅
    L3: Validate Order ✅ (4 rules found)
    L3: Approve Order ✅ (manager only, >$500)
  L2: Order Fulfillment
    L3: Stock Allocation ✅
    L3: Send Confirmation ✅

L1: Business Intelligence
  L2: Sales Reporting
    L3: Monthly Sales Report ✅ (from DB views)
    L3: Revenue by Category ✅
```

### Extracted Business Rules (Partial)

```
BR001: Blocked Customer Check
  Type: Validation
  Source: OrderService.java:12
  Rule: IF customer.status = "BLOCKED" THEN reject order
  Priority: Critical

BR002: Loyalty Discount
  Type: Calculation
  Source: OrderService.java:18 + config
  Rule: IF customer.totalOrders > 10 THEN apply 10% discount
  Config: loyalty.order.threshold = 10
  Priority: High

BR003: High Value Approval
  Type: Approval
  Source: OrderService.java:25 + config
  Rule: IF order.amount > $500 THEN send to manager for approval
  Config: max.order.amount = 500
  Priority: Critical

BR004: Stock Validation
  Type: Validation
  Source: OrderService.java:8
  Rule: IF book.stock < order.quantity THEN reject order
  Priority: Critical
```

### Extracted Process Model (Partial)

```
Process: Book Purchase Order
Trigger: Customer clicks "Place Order"
Actors: Customer, System, Manager

Step 1: Customer enters order details
  Actor: Customer
  
Step 2: System validates customer status
  Decision: Is customer BLOCKED?
    Yes → Reject with message
    No → Continue to Step 3
  
Step 3: System checks book stock
  Decision: Stock sufficient?
    Yes → Continue to Step 4
    No → Reject with message
  
Step 4: System applies loyalty discount
  Rule BR002: If 10+ prior orders → 10% discount
  
Step 5: System checks order amount
  Decision: Amount > $500?
    Yes → Send to manager (Step 6a)
    No → Auto-confirm (Step 6b)
  
Step 6a: Manager approves order
  Actor: Manager
  
Step 6b: System auto-confirms
  Actor: System
  
Step 7: System updates stock and sends confirmation
  Actor: System
  Output: Email to customer
  
End: Order complete
```

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | May 27, 2026 | BA Team | Initial document |

---

**END OF DOCUMENT**

For questions or implementation support, contact: ba-pipeline@company.com
