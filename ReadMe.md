# GetProcessingSteps

This example demonstrates how to read processing steps from a PDF using Mako's `IPDFObject` APIs. These are APIs that can access virtually any element within a PDF, and particularly useful when there is no dedicated API for the object in question.

A brief **Processing Steps** primer is provided below.

## How this sample workflows

First, the document root is obtained with this code:

```C#
    // Get the root object
    var rootObject = document.readPdfObject(IPDFReference.Null());
```

From here, the `Catalog` is obtained with which to look up the `OCProperties` key. (If this key does not exist, then the PDF has no optional content and therefore, no processing steps.)

```C#
    // Get the catalog dictionary
    var catalog = IPDFDictionary.fromRCObject(rootObject);

    // Find a dictionary entry 
    IPDFObject OCGs = catalog.get("OCProperties");
    if (OCGs is null)
    {
        Console.WriteLine("No OCProperties entry found");
        return 1;
    }
```

A recursive method is then called to display the contents of the object and its children.

```C#
    // Display the object & its children
    DisplayObject(document, "OCProperties", OCGs, 0);
```

`DisplayObject()` simply displays the object contents to the console.

### Sample output

When run with the GWG example `Patch PS-001-01G.pdf` the output looks like this:

```Plain
Opening Patch PS-001-01G.pdf...
Dictionary (OCProperties): 2 elements
--Array (OCGs): 3 elements
----Dictionary (0): 3 elements
------Type: OCG
------Name: Test Processing Step
------Dictionary (GTS_Metadata): 2 elements
--------GTS_ProcStepsType: Cutting
--------GTS_ProcStepsGroup: Structural
----Dictionary (1): 3 elements
------Type: OCG
------Name: Base Processing Step
------Dictionary (GTS_Metadata): 2 elements
--------GTS_ProcStepsType: Punching
--------GTS_ProcStepsGroup: Structural
----Dictionary (2): 2 elements
------Type: OCG
------Name: Base Artwork
--Dictionary (D): 4 elements
----Name: Default
----Array (ON): 3 elements
------Dictionary (0): 3 elements
--------Type: OCG
--------Name: Test Processing Step
--------Dictionary (GTS_Metadata): 2 elements
----------GTS_ProcStepsType: Cutting
----------GTS_ProcStepsGroup: Structural
------Dictionary (1): 3 elements
--------Type: OCG
--------Name: Base Processing Step
--------Dictionary (GTS_Metadata): 2 elements
----------GTS_ProcStepsType: Punching
----------GTS_ProcStepsGroup: Structural
------Dictionary (2): 2 elements
--------Type: OCG
--------Name: Base Artwork
----Array (OFF): 0 elements
----Array (Order): 3 elements
------Dictionary (0): 3 elements
--------Type: OCG
--------Name: Test Processing Step
--------Dictionary (GTS_Metadata): 2 elements
----------GTS_ProcStepsType: Cutting
----------GTS_ProcStepsGroup: Structural
------Dictionary (1): 3 elements
--------Type: OCG
--------Name: Base Processing Step
--------Dictionary (GTS_Metadata): 2 elements
----------GTS_ProcStepsType: Punching
----------GTS_ProcStepsGroup: Structural
------Dictionary (2): 2 elements
--------Type: OCG
--------Name: Base Artwork
```

## Processing Steps Primer

### 📌 What “Processing Steps” Are

Processing Steps in PDF are a standardized mechanism for describing production-specific information that goes beyond the visual appearance of the page. They capture instructions or metadata about how the page or its content should be manufactured, processed, or finished.

Examples include:

* **Cutting and folding lines** for packaging
* **Varnish or coating masks**
* **Die cutting paths**
* **Bleed or trimming information**
* **Registration marks, proofing data, or printer calibration patches**

These are not normally meant for end-user viewing, but are crucial in professional publishing and packaging workflows.

### 📌 Why They Were Needed

As PDF became the standard in graphic arts and print production (ISO 15930 – PDF/X), it was increasingly used for packaging, labeling, and industrial printing.

The problem:

* Printers and packaging converters needed to include technical information (cut paths, fold lines, safety zones, ink coverage, etc.) inside the PDF.
* Before standardization, this was often handled via proprietary layers or extra annotation conventions, leading to incompatibility and ambiguity.
* The packaging industry ([CIP4](https://www.cip4.org/), [Ghent Workgroup](https://gwg.org/ghent-processing-stepts-output-suite/), and others) pushed for a standardized way to describe this data inside the PDF itself.

Thus, the [ISO 19593 standard](https://www.iso.org/standard/65428.html) (“Processing Steps for Packaging and Labeling”) was developed and later incorporated into the PDF 2.0 standard ([ISO 32000-2](https://www.iso.org/standard/75839.html)).

### 📌 How They Work in PDF

Processing Steps are represented using:

1. **Optional Content Groups (OCGs, i.e. PDF “layers”)**
    * Processing Steps are usually stored as layers, so they can be toggled on/off in viewers.
    * Each Processing Step layer is tagged with metadata describing its role.

2. **Standardized Metadata**
    * Each Processing Step has a **`/Category`** (e.g. `/Cut`, `/Fold`, `/Varnish`, `/WhiteInk` etc.).
    * This metadata makes it machine-readable and consistent across tools.

3. **Separation from Artwork**
    * The visual artwork (what the consumer sees) is separate from processing instructions.
    * This ensures **no accidental printing** of die lines or technical guides.

### 📌 Purpose & Benefits

* **Interoperability**: Printers, packaging converters, and prepress systems can reliably exchange files without misinterpreting technical marks.
* **Automation**: Workflow automation (cutting machines, folding systems, digital finishing devices) can directly extract the necessary instructions.
* **Standardization**: Replaces ad-hoc “drawn lines” and vendor-specific metadata with ISO-defined categories.
* **Safety**: Prevents costly mistakes like printing cutting lines on the actual product.
