/* --------------------------------------------------------------------------------
 *  <copyright file="Program.cs" company="Global Graphics Software Ltd">
 *    Copyright (c) 2025 Global Graphics Software Ltd. All rights reserved.
 *  </copyright>
 *  <summary>
 *    This example is provided on an "as is" basis and without warranty of any kind.
 *    Global Graphics Software Ltd. does not warrant or make any representations
 *    regarding the use or results of use of this example.
 *  </summary>
 * ---------------------------------------------------------------------------------
 */

using JawsMako;

namespace GetProcessingSteps;

internal class Program
{
    static int Main()
    {
        try
        {
            var testFilepath = @"..\..\..\..\TestFiles\";
            var testFile = $"{testFilepath}Patch PS-001-01G.pdf";

            var mako = IJawsMako.create();
            IJawsMako.enableAllFeatures(mako);

            // Create our input
            IPDFInput input = IPDFInput.create(mako);

            Console.WriteLine($"Opening {new FileInfo(testFile).Name}...");

            // Get the assembly from the input.
            IDocumentAssembly assembly = input.open(testFile);

            // Get the document
            IDocument document = assembly.getDocument();

            // Get the root object
            var rootObject = document.readPdfObject(IPDFReference.Null());
            if (rootObject.getType() != ePDFObjectType.ePOTDictionary)
            {
                Console.WriteLine("Catalog could not be retrieved. Exiting...");
                return 1;
            }

            // Get the catalog dictionary
            var catalog = IPDFDictionary.fromRCObject(rootObject);

            // Find a dictionary entry 
            IPDFObject OCGs = catalog.get("OCProperties");
            if (OCGs is null)
            {
                Console.WriteLine("No OCProperties entry found");
                return 1;
            }

            // Display the object & its children
            DisplayObject(document, "OCProperties", OCGs, 0);

        }
        catch (MakoException e)
        {
            Console.WriteLine($"Exception thrown: {e.m_errorCode}: {e.m_msg}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception thrown: {e}");
        }

        return 0;
    }

    static void DisplayObject(IDocument document, string name, IPDFObject obj, int indentLevel)
    {
        var objType = obj.getType();

        // If it's a reference, we need to resolve it first
        if (objType == ePDFObjectType.ePOTReference)
        {
            obj = document.readPdfObject(IPDFReference.fromRCObject(obj));
        }
        else if (objType == ePDFObjectType.ePOTFarReference)
        {
            var farRefResult = document.lookupFarReference(IPDFFarReference.fromRCObject(obj));
            obj = farRefResult.first; // The referenced object
        }

        Console.Write(new string('-', indentLevel * 2));

        objType = obj.getType();
        switch (objType)
        {
            case ePDFObjectType.ePOTString:
                var str = IPDFString.fromRCObject(obj);
                Console.WriteLine($"Name: {str.getValue()}");
                break;

            case ePDFObjectType.ePOTInteger:
                var integer = IPDFInteger.fromRCObject(obj);
                Console.WriteLine($"Integer: {integer.getValue()}");
                break;

            case ePDFObjectType.ePOTReal:
                var real = IPDFReal.fromRCObject(obj);
                Console.WriteLine($"Real: {real.getValue()}");
                break;

            case ePDFObjectType.ePOTName:
                var pdfName = IPDFName.fromRCObject(obj);
                Console.WriteLine($"{name}: {pdfName.getValue()}");
                break;

            case ePDFObjectType.ePOTOperator:
                var pdfOperator = IPDFOperator.fromRCObject(obj);
                Console.WriteLine($"Operator: {pdfOperator.getValue()}");
                break;

            case ePDFObjectType.ePOTBoolean:
                var boolean = IPDFBoolean.fromRCObject(obj);
                Console.WriteLine($"Boolean: {boolean.getValue()}");
                break;
            
            case ePDFObjectType.ePOTNull:
                Console.WriteLine("Null");
                break;

            case ePDFObjectType.ePOTStream:
                var stream = IPDFStream.fromRCObject(obj);
                var streamLength = stream.getStream().IRAStream_length();
                Console.Write("(Stream) Length: ");
                Console.WriteLine(streamLength < 0 ? "Unknown" : streamLength);
                break;
            
            case ePDFObjectType.ePOTArray:
                var array = IPDFArray.fromRCObject(obj);
                Console.WriteLine($"Array ({name}): {array.getSize()} elements");
                var arraySize = array.getSize();
                var arrayIndent = indentLevel + 1;
                for (uint i = 0; i < arraySize; i++)
                {
                    DisplayObject(document, $"{i}", array.get(i), arrayIndent);
                }
                break;
            
            case ePDFObjectType.ePOTDictionary:
                var dictionary = IPDFDictionary.fromRCObject(obj);
                var dictSize = dictionary.getSize();
                Console.WriteLine($"Dictionary ({name}): {dictionary.getSize()} elements");
                var dictIndent = indentLevel + 1;
                for (uint i = 0; i < dictSize; i++)
                {
                    var key = dictionary.getKeyAtIndex(i);
                    var value = dictionary.getValueAtIndex(i);
                    DisplayObject(document, key.getValue(), value, dictIndent);
                }
                break;

            default:
                Console.WriteLine($"Unknown type: {objType}");
                break;
        }
    }
}