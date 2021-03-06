﻿using System;
using System.IO;
using System.Xml;
using static System.Console;
using static System.Environment;
using static System.IO.Path;
using System.IO.Compression;

namespace WorkingWithStreams
{
    class Program
    {
        //define an array of Viper pilot call signs
        static string[] callsigns = new string[] { "Husker", "Starbuck", "Apollo", "Boomer", "Bulldog", "Athena", "Helo", "Racetrack" };

        static void WorkWithText()
        {
            //define a file to write to 
            string textFile = Combine(CurrentDirectory, "stream.txt");

            // create a text file and return a helper writer
            StreamWriter text = File.CreateText(textFile);

            //enumerate the strings,writing each one
            //to the stream on a separate line
            foreach (string item in callsigns)
            {
                text.WriteLine(item);
            }
            text.Close();//release resources,the process need to be shut down.

            //output the contexts of the file to the console
            WriteLine($"{textFile} contains {new FileInfo(textFile).Length} bytes.");
            WriteLine(File.ReadAllText(textFile));
        }

        static void WorkWithXML()
        {
            FileStream xmlFileStream = null;
            XmlWriter xml = null;
            try
            {
                //define a file to write to
                string xmlFile = Combine(CurrentDirectory, "streams.xml");

                //create a file streams
                xmlFileStream = File.Create(xmlFile);

                //Wrap the file stream in an XML writer helper
                //and automatically indent nested elements
                xml = XmlWriter.Create(xmlFileStream, new XmlWriterSettings { Indent = true });

                //write the XML declration
                xml.WriteStartDocument();

                //write a root element 
                xml.WriteStartElement("callsigns");

                //enumerate the strings writing each one to the stream
                foreach (string item in callsigns)
                {
                    xml.WriteElementString("callsign", item);
                }

                //write the close root element
                xml.WriteEndElement();

                //close helper and stream
                xml.Close();
                xmlFileStream.Close();

                // output all the contents of the file to the Console 
                WriteLine($"{xmlFile} contains {new FileInfo(xmlFile).Length} bytes.");
                WriteLine(File.ReadAllText(xmlFile));
            }
            catch (Exception ex)
            {
                // if the path doesn't exist the exception will be caught
                WriteLine($"{ex.GetType()} says {ex.Message}");
            }
            finally
            {
                if (xml != null)
                {
                    xml.Dispose();
                    WriteLine("The XML writer's unmanaged resources have been disposed.");
                }
                if (xmlFileStream != null)
                {
                    xmlFileStream.Dispose();
                    WriteLine("The file stream's unmanaged resources have been disposed.");
                }
            }
        }
        static void WorkWithCompression()
        {
            // compress the XML output
            string gzipFilePath = Combine(CurrentDirectory,"Stream.gzip");
            FileStream gzipFile = File.Create(gzipFilePath);
            using (GZipStream compressor = new GZipStream(gzipFile, CompressionMode.Compress))
            {
                using (XmlWriter xmlGzip = XmlWriter.Create(compressor))
                {
                    xmlGzip.WriteStartDocument();
                    xmlGzip.WriteStartElement("callsigns");
                    foreach (string item in callsigns)
                    {
                        xmlGzip.WriteElementString("callsigns",item);
                    }
                }// also closes the underlying stream 

                // output all the contents of the compressed file to the Console 
                WriteLine($"{gzipFilePath} contains  { new FileInfo(gzipFilePath).Length} bytes."); 
                WriteLine(File.ReadAllText(gzipFilePath));

                //read a compressed file
                WriteLine("Reading the compressed XML File:");
                gzipFile = File.Open(gzipFilePath, FileMode.Open);
                using (GZipStream decompressor = new GZipStream(gzipFile, CompressionMode.Decompress))
                {
                    using (XmlReader reader = XmlReader.Create(decompressor))
                    {
                        while (reader.Read())
                        {
                            //check if we are currently on an element node named callsign 
                            if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "callsign"))
                            {
                                reader.Read();//move to the Text node inside the element
                                WriteLine($"{reader.Value}");// read its value
                            }
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            //WorkWithText();
            WorkWithXML();
            WorkWithCompression();
        }
    }
}
