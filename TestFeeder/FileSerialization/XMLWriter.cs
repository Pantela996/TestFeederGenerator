using FTN;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestFeeder.FileSerialization
{

    // Logic explanation: Since we are not working with classic RDF, aproach is parsing existing entity model that is in form Dict<Type, Dict<string,IdentifiedObject),
    // and converting values, since with this method all properties are getting written, we are filtering them through List filter for each entity (eg. allowed_properties_circuit)
    public class XMLWriter : IDataProvider
    {
        private Dictionary<Type, Dictionary<string, IdentifiedObject>> content;

        private string output;

        private List<string> allowed_entities = new List<string>()
        {
            "Circuit",
            "ConnectivityNode",
            "Terminal",
            "PowerTransformer",
            "PowerTransformerEnd",
            "PerLengthSequenceImpedance",
            "WireInfo",
            "UsagePoint",
            "PSRType",
            "WireInfo"
        };

        private List<string> allowed_properties_circuit = new List<string>()
        {
            "MRID",
            "Name",
            "PSRType"
        };

        private List<string> allowed_properties_connectivity_node = new List<string>()
        {
            "MRID",
            "Name",
            "ConnectivityNodeContainer"
        };

        private List<string> allowed_properties_usage_point = new List<string>()
        {
            "MRID",
            "Name",
            "ServiceLocation",
            "RatedPower"
        };

        private List<string> allowed_properties_psr_type = new List<string>()
        {
            "MRID",
            "Name"
        };

        private List<string> allowed_properties_pli = new List<string>()
        {
            "MRID",
            "Name",
            "B0ch",
            "Bch",
            "R0",
            "R",
            "X0",
            "X"
        };


        private List<string> allowed_properties_wire_info = new List<string>()
        {
            "MRID",
            "Name",
            "Gmr",
            "Material",
            "RDC20",
            "Radius",
            "RatedCurrent",
            "SizeDescription",
        };

        private List<string> allowed_properties_ac_line = new List<string>()
        {
            "MRID",
            "Name",
            "PSRType",
            "AssetDatasheet",
            "EquipmentContainer",
            "Length",
            "PerLengthImpedance"
        };

        private List<string> allowed_properties_power_transformer = new List<string>()
        {
            "MRID",
            "Name",
            "PSRType",
            "EquipmentContainer"
        };

        private List<string> allowed_properties_power_transformer_end = new List<string>()
        {
            "MRID",
            "Name",
            "PSRType",
            "Terminal",
            "EndNumber",
            "Grounded",
            "PowerTransformer",
            "ConnectionKind",
            "PhaseAngleClock",
            "RatedS",
            "RatedU"
        };

        private List<string> allowed_properties_terminal = new List<string>()
        {
            "MRID",
            "Name",
            "SequenceNumber",
            "ConductingEquipment",
            "ConnectivityNode",
            "Phases"
        };

        private List<string> allowed_properties_sync_machine = new List<string>()
        {
            "MRID",
            "Name",
            "EquipmentContainer"
        };



        public string Output
        {
            get { return output; }
            set { output = value; }
        }



        public Dictionary<Type, Dictionary<string, IdentifiedObject>> Content
        {
            get { return content; }
            set { content = value; }
        }


        public XMLWriter(Dictionary<Type, Dictionary<string, IdentifiedObject>> content)
        {
            this.content = content;
        }


        public void WriteToFile(string filename, object file=null)
        {
            output = "";
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF - 8\"?>");
            sb.Append("\n");

            //this line is not good probably
            sb.Append("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\" xmlns:cim=\"http://iec.ch/TC57/2017/CIM-schema-cim17#\" xmlns:sedms= \"http://www.schneider-electric-dms.com/CIM17v38/2017/extension\"xml:base= \"http://iec.ch/TC57/2017/CIM-schema-cim17#\" >");
            sb.Append("\n");
            sb.Append("\n");

            GenerateStringBasedOn(content, sb);

            sb.Append("\n");
            sb.Append("\n");
            sb.Append("</rdf:RDF>");

            File.WriteAllText(filename + ".xml", sb.ToString());

        }

        private void GenerateStringBasedOn(Dictionary<Type, Dictionary<string, IdentifiedObject>> content, StringBuilder sb)
        {
            sb.Append("\n");
            sb.Append("\t");
            foreach (Type type in content.Keys)
            {

                foreach (IdentifiedObject id in content[type].Values)
                {
                    sb.Append("<cim:");
                    sb.Append(type.Name);

                    sb.Append(" rdf:ID=\"");
                    sb.Append(id.MRID);
                    sb.Append("\">");
                    //sb.Append("\n");
                    foreach (PropertyInfo propertyInfo in id.GetType().GetProperties())
                    {

                        // property with not defined values are not considered
                        if (propertyInfo.GetValue(id) == null)
                        {
                            continue;
                        }

                        //filtering mentioned at start
                        if (CheckProperty(propertyInfo, type.Name) == false)
                        {
                            continue;
                        }

                        //converting property value to be in CIMXML standards
                        string updated_property_info_name = UpdatePropertyName(propertyInfo.Name);

                        sb.Append("\n");
                        sb.Append("\t");
                        sb.Append("\t");



                        sb.Append("<cim:");

                        if (updated_property_info_name != "")
                        {
                            sb.Append(updated_property_info_name);
                        }
                        else
                        {
                            sb.Append(propertyInfo.Name);
                        }


                        //since we are working with rdfs, we can define relations, so instead of object value we set reference through mrid
                        string property_id = CheckIfPropertyNeedsId(updated_property_info_name, propertyInfo.GetValue(id));
                        if (property_id != "")
                        {
                            sb.Append(" ");
                            sb.Append("rdf:resource=\"");
                            sb.Append("#" + property_id);
                            sb.Append("\"");

                        }


                        sb.Append(">");
                        if (property_id == "")
                        {
                            sb.Append(propertyInfo.GetValue(id).ToString());
                        }

                        sb.Append("</cim:");
                        if (updated_property_info_name != "")
                        {
                            sb.Append(updated_property_info_name);
                        }
                        else
                        {
                            sb.Append(propertyInfo.Name);
                        }
                        sb.Append(">");
                        sb.Append("\t");
                    }
                    sb.Append("\n");
                    sb.Append("\t");
                    sb.Append("</cim:");
                    sb.Append(type.Name);
                    sb.Append(">");
                    sb.Append("\n");
                    sb.Append("\t");
                }


            }
        }

        private string CheckIfPropertyNeedsId(string updated_property_info_name, object data)
        {


            switch (updated_property_info_name)
            {
                case "ConnectivityNode.ConnectivityNodeContainer":
                    Circuit c = (Circuit)data;
                    return c.MRID;
                case "UsagePoint.ServiceLocation":
                    ServiceLocation sl = (ServiceLocation)data;
                    return sl.MRID;
                case "PowerSystemResource.PSRType":
                    PSRType psr = (PSRType)data;
                    return psr.Name;
                case "ACLineSegment.PerLengthImpedance":
                    PerLengthImpedance pli = (PerLengthImpedance)data;
                    return pli.MRID;
                case "Equipment.EquipmentContainer":
                    EquipmentContainer eq = (EquipmentContainer)data;
                    return eq.MRID;
                case "PowerSystemResource.AssetDatasheet":
                    AssetInfo ad = (AssetInfo)data;
                    return ad.MRID;
                case "TransformerEnd.Terminal":
                    Terminal t = (Terminal)data;
                    return t.MRID;
                case "PowerTransformerEnd.PowerTransformer":
                    PowerTransformer pst = (PowerTransformer)data;
                    return pst.MRID;
                case "Terminal.ConductingEquipment":
                    ConductingEquipment ce = (ConductingEquipment)data;
                    return ce.MRID;
                case "Terminal.ConnectivityNode":
                    ConnectivityNode cn = (ConnectivityNode)data;
                    return cn.MRID;
            }


            return "";
        }

        private string UpdatePropertyName(string name)
        {
            switch (name)
            {
                case "MRID":
                    return "IdentifiedObject.mRID";
                case "Name":
                    return "IdentifiedObject.name";


                case "PSRType":
                    return "PowerSystemResource.PSRType";
                case "ConnectivityNodeContainer":
                    return "ConnectivityNode.ConnectivityNodeContainer";
                case "ServiceLocation":
                    return "UsagePoint.ServiceLocation";


                case "B0ch":
                    return "PerLengthSequenceImpedance.b0ch";
                case "Bch":
                    return "PerLengthSequenceImpedance.bch";
                case "R":
                    return "PerLengthSequenceImpedance.r";
                case "R0":
                    return "PerLengthSequenceImpedance.r0";
                case "X":
                    return "PerLengthSequenceImpedance.x";
                case "X0":
                    return "PerLengthSequenceImpedance.x0";
                case "Gmr":
                    return "WireInfo.gmr";
                case "Material":
                    return "WireInfo.material";
                case "RDC20":
                    return "WireInfo.rDC20";
                case "Radius":
                    return "WireInfo.radius";
                case "RatedCurrent":
                    return "WireInfo.ratedCurrent";
                case "SizeDescription":
                    return "WireInfo.sizeDescription";


                case "Length":
                    return "Conductor.length";
                case "PerLengthImpedance":
                    return "ACLineSegment.PerLengthImpedance";
                case "EquipmentContainer":
                    return "Equipment.EquipmentContainer";
                case "AssetDatasheet":
                    return "PowerSystemResource.AssetDatasheet";

                case "PowerTransformer":
                    return "PowerTransformerEnd.PowerTransformer";
                case "Terminal":
                    return "TransformerEnd.Terminal";
                case "ConnectionKind":
                    return "PowerTransformerEnd.connectionKind";
                case "PhaseAngleClock":
                    return "PowerTransformerEnd.phaseAngleClock";
                case "RatedS":
                    return "PowerTransformerEnd.ratedS";
                case "RatedU":
                    return "PowerTransformerEnd.ratedU";
                case "EndNumber":
                    return "TransformerEnd.endNumber";
                case "Grounded":
                    return "TransformerEnd.grounded";

                case "SequenceNumber":
                    return "ACDCTerminal.sequenceNumber";
                case "ConductingEquipment":
                    return "Terminal.ConductingEquipment";
                case "ConnectivityNode":
                    return "Terminal.ConnectivityNode";
                case "Phases":
                    return "Terminal.Phases";



                default:
                    return "";
            }
        }

        private bool CheckProperty(PropertyInfo propertyInfo, string name)
        {
            switch (name)
            {
                case "Circuit":

                    if (!allowed_properties_circuit.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;

                case "ConnectivityNode":
                    if (!allowed_properties_connectivity_node.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;

                case "UsagePoint":
                    if (!allowed_properties_usage_point.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;
                case "PSRType":
                    if (!allowed_properties_psr_type.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;

                case "PerLengthSequenceImpedance":
                    if (!allowed_properties_pli.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;

                case "WireInfo":
                    if (!allowed_properties_wire_info.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;

                case "ACLineSegment":
                    if (!allowed_properties_ac_line.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;
                case "PowerTransformer":
                    if (!allowed_properties_power_transformer.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;
                case "PowerTransformerEnd":
                    if (!allowed_properties_power_transformer_end.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;
                case "Terminal":
                    if (!allowed_properties_terminal.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;
                case "SynchronousMachine":
                    if (!allowed_properties_sync_machine.Contains(propertyInfo.Name))
                    {
                        return false;
                    }
                    break;

            }

            return true;
        }

        public object ReadFromFile(string filename)
        {
            return null;
        }
    }
}
