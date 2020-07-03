using FTN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using TestFeeder.Models;
using TestFeederGenerator.Models;

namespace TestFeeder.ViewModels
{
    public class CIMObjectModelLoader
    {

        private Dictionary<Type, Dictionary<string, IdentifiedObject>> globalComponentDictionary;
        private List<TerminalPair> terminalPairsContainer;
        private Dictionary<string, PerLengthImpedance> perLengthImpedanceContainer;
        private Dictionary<string, WireInfo> wireInfoContainer;
        private Dictionary<string, SpotLoad> usagePointContainer;
        private Dictionary<string, DataVertexTransformer> powerTransformerEnding;
        private Circuit circuit;

        public Dictionary<Type, Dictionary<string, IdentifiedObject>> CreateObjectModel(Dictionary<string, DataVertex> globalVertices, Dictionary<string, List<DataEdge>> globalEdges, Dictionary<string, CableConfiguration> globalCableConfiguration, Dictionary<string, SpotLoad> globalSpotLoads)
        {
            globalComponentDictionary = new Dictionary<Type, Dictionary<string, IdentifiedObject>>();
            terminalPairsContainer = new List<TerminalPair>();
            perLengthImpedanceContainer = new Dictionary<string, PerLengthImpedance>();
            wireInfoContainer = new Dictionary<string, WireInfo>();
            usagePointContainer = new Dictionary<string, SpotLoad>();
            powerTransformerEnding = new Dictionary<string, DataVertexTransformer>();

            PSRType local_psr_pt = new PSRType();

            circuit = new Circuit();
            string mrid = Guid.NewGuid().ToString();
            PSRType psrTypeCircuit = new PSRType() { MRID = "Feeder", Name = "Feeder" };
            circuit.PSRType = psrTypeCircuit;

            circuit.ID = mrid;
            circuit.MRID = mrid;
            circuit.Name = "Feeder_36";

            addComponentToGlobalDictionary(circuit, circuit.GetType());
            addComponentToGlobalDictionary(psrTypeCircuit, psrTypeCircuit.GetType());

            Dictionary<string, ConnectivityNode> connectivityNodeContainer = new Dictionary<string, ConnectivityNode>();
           

            

            foreach (DataVertex dataVertex in globalVertices.Values)
            {


                
                if (dataVertex.typeOfVertex == DataVertex.TypeOfVertex.TRANSFORMER_VERTEX)
                {
                    PowerTransformer powerTransformer = new PowerTransformer();

                    DataVertexTransformer dvt = (DataVertexTransformer)dataVertex;

                    string power_transformer_mrid = Guid.NewGuid().ToString();
                    powerTransformer.ID = power_transformer_mrid;
                    powerTransformer.MRID = power_transformer_mrid;
                    powerTransformer.Name = "2 winding power transformer";
                    local_psr_pt = new PSRType() { Name = "Consumer Transformer", MRID = "Consumer Transformer" };
                    powerTransformer.PSRType = local_psr_pt;
                    powerTransformer.EquipmentContainer = circuit;
                    addComponentToGlobalDictionary(local_psr_pt, local_psr_pt.GetType());

                  


                    ConnectivityNode w1T_cn = new ConnectivityNode();
                    string connectivity_node_mrid = Guid.NewGuid().ToString();
                    w1T_cn.ID = connectivity_node_mrid;
                    w1T_cn.MRID = connectivity_node_mrid;
                    w1T_cn.ConnectivityNodeContainer = circuit;
                    if ((dataVertex as DataVertexTransformer).Line_from == null)
                    {
                        continue;
                    };
                    w1T_cn.Name = (dataVertex as DataVertexTransformer).Line_from;
                    connectivityNodeContainer.Add((dataVertex as DataVertexTransformer).Line_from, w1T_cn);
                    addComponentToGlobalDictionary(w1T_cn, w1T_cn.GetType());

                    ConnectivityNode w2T_cn = new ConnectivityNode();
                    connectivity_node_mrid = Guid.NewGuid().ToString();
                    w2T_cn.ID = connectivity_node_mrid;
                    w2T_cn.MRID = connectivity_node_mrid;
                    w2T_cn.ConnectivityNodeContainer = circuit;
                    w2T_cn.Name = (dataVertex as DataVertexTransformer).Line_to;
                    connectivityNodeContainer.Add((dataVertex as DataVertexTransformer).Line_to, w2T_cn);
                    if ((dataVertex as DataVertexTransformer).Line_to == null)
                    {
                        continue;
                    };
                    addComponentToGlobalDictionary(w2T_cn, w2T_cn.GetType());

                    Terminal w1T = new Terminal();
                    w1T.MRID = power_transformer_mrid + ".W1.T";
                    w1T.ID = power_transformer_mrid + ".W1.T";
                    w1T.SequenceNumber = 1;
                    w1T.ConductingEquipment = powerTransformer;
                    w1T.ConnectivityNode = w1T_cn;
                    w1T.Phases = PhaseCode.s2N;
                    w1T.Name = "Transformer end terminal 1";

                    

                    Terminal w2T = new Terminal();
                    w2T.MRID = power_transformer_mrid + ".W2.T";
                    w2T.ID = power_transformer_mrid + ".W2.T";
                    w2T.SequenceNumber = 2;
                    w2T.ConductingEquipment = powerTransformer;
                    w2T.ConnectivityNode = w2T_cn;
                    w2T.Phases = PhaseCode.s2N;
                    w2T.Name = "Transformer end terminal 2";

                    terminalPairsContainer.Add(new TerminalPair() { terminalA = w1T, terminalB = w2T});

                    

                    addComponentToGlobalDictionary(w1T, w1T.GetType());
                    addComponentToGlobalDictionary(w2T, w2T.GetType());


                    PowerTransformerEnd powerTransformerEnd1 = new PowerTransformerEnd();

                    powerTransformerEnd1.ID = power_transformer_mrid + ".W1";
                    powerTransformerEnd1.MRID = power_transformer_mrid + ".W1";
                    powerTransformerEnd1.Name = dvt.NameA;
                    powerTransformerEnd1.Terminal = w1T;
                    powerTransformerEnd1.Grounded = false;
                    powerTransformerEnd1.EndNumber = 1;
                    powerTransformerEnd1.PowerTransformer = powerTransformer;
                    powerTransformerEnd1.ConnectionKind = WindingConnection.D;
                    powerTransformerEnd1.PhaseAngleClock = 0;
                    powerTransformerEnd1.RatedS = (float)dvt._kVA_A;
                    powerTransformerEnd1.RatedU = (float)dvt._kV_LowA;


                    PowerTransformerEnd powerTransformerEnd2 = new PowerTransformerEnd();

                    powerTransformerEnd2.ID = power_transformer_mrid + ".W2";
                    powerTransformerEnd2.MRID = power_transformer_mrid + ".W2";
                    powerTransformerEnd2.Name = dvt.NameB;
                    powerTransformerEnd2.Terminal = w2T;
                    powerTransformerEnd2.Grounded = false;
                    powerTransformerEnd2.EndNumber = 1;
                    powerTransformerEnd2.PowerTransformer = powerTransformer;
                    powerTransformerEnd2.ConnectionKind = WindingConnection.D;
                    powerTransformerEnd2.PhaseAngleClock = 0;
                    powerTransformerEnd2.RatedS = (float)dvt._kVA_B;
                    powerTransformerEnd2.RatedU = (float)dvt._kV_LowB;

                    addComponentToGlobalDictionary(powerTransformer, powerTransformer.GetType());
                    addComponentToGlobalDictionary(powerTransformerEnd1, powerTransformerEnd1.GetType());
                    addComponentToGlobalDictionary(powerTransformerEnd2, powerTransformerEnd2.GetType());
                }


                if (dataVertex.typeOfVertex == DataVertex.TypeOfVertex.REGULATOR_VERTEX)
                {
                    string sync_machine = Guid.NewGuid().ToString();
                    PSRType psr_sync_machine = new PSRType()
                    {
                        MRID = "Generator",
                        Name = "Generator"
                    };
                    SynchronousMachine sm = new SynchronousMachine()
                    {
                        Name = dataVertex.Text,
                        MRID = sync_machine,
                        ID = sync_machine,
                        EquipmentContainer = circuit
                    };
                    addComponentToGlobalDictionary(sm, sm.GetType());
                    addComponentToGlobalDictionary(psr_sync_machine, psr_sync_machine.GetType());
                }
            }


            foreach (List<DataEdge> dataEdgeCollection in globalEdges.Values)
            {

                foreach (DataEdge dataEdge in dataEdgeCollection)
                {



                    string acLineSegment_mrid = Guid.NewGuid().ToString();


                    ConnectivityNode T1_cn = new ConnectivityNode();
                    string connectivity_node_mrid = Guid.NewGuid().ToString();
                    T1_cn.ID = connectivity_node_mrid;
                    T1_cn.MRID = connectivity_node_mrid;
                    T1_cn.ConnectivityNodeContainer = circuit;
                    T1_cn.Name = (dataEdge.Source as DataVertex).Element_id;
                    if (connectivityNodeContainer.ContainsKey((dataEdge.Source as DataVertex).Element_id) == false)
                    {
                        connectivityNodeContainer.Add(dataEdge.Source.Element_id, T1_cn);
                        addComponentToGlobalDictionary(T1_cn, T1_cn.GetType());
                    }
                    
                  

                    ConnectivityNode T2_cn = new ConnectivityNode();
                    connectivity_node_mrid = Guid.NewGuid().ToString();
                    T2_cn.ID = connectivity_node_mrid;
                    T2_cn.MRID = connectivity_node_mrid;
                    T2_cn.ConnectivityNodeContainer = circuit;
                    T2_cn.Name = (dataEdge.Target as DataVertex).Element_id;
                    if (connectivityNodeContainer.ContainsKey((dataEdge.Target as DataVertex).Element_id) == false)
                    {
                        connectivityNodeContainer.Add(dataEdge.Target.Element_id, T2_cn);
                        addComponentToGlobalDictionary(T2_cn, T2_cn.GetType());
                    }
                    


                    Terminal T1 = new Terminal();
                    //string terminal_mrid = Guid.NewGuid().ToString();
                    T1.ID = acLineSegment_mrid + ".T1";
                    T1.MRID = acLineSegment_mrid + ".T1";
                    T1.Name = dataEdge.Source.Element_id;
                    T1.Phases = PhaseCode.ABC;
                    ACDCTerminal acdc_terminal = new ACDCTerminal() { SequenceNumber = 1 };
                    T1.SequenceNumber = acdc_terminal.SequenceNumber;
                    T1.ConnectivityNode = T1_cn;



                    Terminal T2 = new Terminal();
                    T2.ID = acLineSegment_mrid + ".T2";
                    T2.MRID = acLineSegment_mrid + ".T2";
                    T2.Name = dataEdge.Target.Element_id;
                    T2.Phases = PhaseCode.ABC;
                    ACDCTerminal acdc_terminal2 = new ACDCTerminal() { SequenceNumber = 2 };
                    T2.SequenceNumber = acdc_terminal2.SequenceNumber;
                    T2.ConnectivityNode = T2_cn;

                    string perLengthImpedance_mrid = Guid.NewGuid().ToString();
                    PerLengthImpedance pli =  createPerLengthImpedanceObject(dataEdge.Configuration, perLengthImpedance_mrid);
                    AssetInfo wi = createWireInfoObject(dataEdge.Configuration, perLengthImpedance_mrid);
                   
                    PSRType acPSRType = new PSRType()
                    {
                        MRID = "Section",
                        Name = "Section"
                    };
                    if (!globalComponentDictionary[acPSRType.GetType()].ContainsKey(acPSRType.Name))
                    {
                        addComponentToGlobalDictionary(acPSRType, acPSRType.GetType());
                    }
                    


                    ACLineSegment acLineSegment = new ACLineSegment()
                    {
                        ID = acLineSegment_mrid,
                        MRID = acLineSegment_mrid,
                        Name = T1.Name.Split(' ').Last() + "-" + T2.Name.Split(' ').Last(),
                        PSRType = acPSRType,
                        EquipmentContainer = circuit,
                        Length = (float)feetsToMeters(dataEdge.Length),
                        PerLengthImpedance = pli,
                        AssetDatasheet = wi
                    };

                    addComponentToGlobalDictionary(acLineSegment, acLineSegment.GetType());


                    TerminalPair terminalPair = new TerminalPair()
                    {
                        terminalA = T1,
                        terminalB = T2
                    };


                    terminalPairsContainer.Add(terminalPair);

                    addComponentToGlobalDictionary(T1, T1.GetType());
                    addComponentToGlobalDictionary(T2, T2.GetType());

                }

            }

            UsagePoint usagePoint = new UsagePoint();


            foreach (DataVertex dv in globalVertices.Values)
            {
                if(dv.typeOfVertex == DataVertex.TypeOfVertex.SPOT_LOAD_VERTEX)
                {
                    SpotLoad sl = (SpotLoad)dv;
                    usagePoint = CreateSpotLoad(sl);
                    addComponentToGlobalDictionary(usagePoint, usagePoint.GetType());
                }
            }




            return globalComponentDictionary;
        }

        private UsagePoint CreateSpotLoad(SpotLoad sl)
        {
            UsagePoint usagePoint = new UsagePoint();
            ServiceLocation serviceLocation = new ServiceLocation();
            string usage_point_mrid = Guid.NewGuid().ToString();
            usagePoint.ID = usage_point_mrid;
            usagePoint.MRID = usage_point_mrid;
            usagePoint.Name = sl.Text;
            serviceLocation = createServiceLocation(serviceLocation);
            usagePoint.ServiceLocation = serviceLocation;
            usagePoint.RatedPower = 0;

            return usagePoint;
        }

        private ServiceLocation createServiceLocation(ServiceLocation serviceLocation)
        {
            string service_location_mrid = Guid.NewGuid().ToString();
            serviceLocation.ID = service_location_mrid;
            serviceLocation.MRID = service_location_mrid;
            serviceLocation.Name = "Service Location: ";


            return serviceLocation;
        }

        private AssetInfo createWireInfoObject(CableConfiguration configuration, string mRID)
        {
            WireInfo wi = new WireInfo();
            if (!wireInfoContainer.ContainsKey(configuration.Name))
            {
                if (configuration.Name != "xfm_1")
                {
                    string wi_mrid = mRID + "_WI";

                    wi.ID = wi_mrid;
                    wi.MRID = wi_mrid;
                    wi.Name = configuration.Name;
                    wi.Material = (WireMaterialKind)((int)configuration.Cable.MaterialKind);
                    wi.SizeDescription = configuration.Cable.SizeDescription;
                    wi.Gmr = (float)0.001;
                    wi.Radius = 0;
                    wi.RatedCurrent = 0;

                    wireInfoContainer.Add(configuration.Name, wi);
                    addComponentToGlobalDictionary(wi, wi.GetType());
                }


                //RADIUS and RATED_CURRENT are going to have values

            }
            else
            {
                wi = wireInfoContainer[configuration.Name];
                return wi;
            }
                

            return wi;

        }

        private PerLengthImpedance createPerLengthImpedanceObject(CableConfiguration configuration, string acLineSegment_mrid)
        {
            PerLengthSequenceImpedance pli = new PerLengthSequenceImpedance();

            if (!perLengthImpedanceContainer.ContainsKey(configuration.Name))
            {

                if (configuration.Name != "xfm_1")
                {
                    string pli_mrid = acLineSegment_mrid + "_PLI";

                    pli.ID = pli_mrid;
                    pli.MRID = pli_mrid;
                    pli.Name = configuration.Name;
                    pli.Bch = (float)configuration.Susceptance;
                    pli.B0ch = 0;
                    pli.R = (float)configuration.Primar_value.Real_part;
                    pli.R0 = 0;
                    pli.X = (float)configuration.Primar_value.Imaginary_part;
                    pli.X0 = 0;

                    perLengthImpedanceContainer.Add(configuration.Name, pli);

                    addComponentToGlobalDictionary(pli, pli.GetType());
                }


            }
            else
            {
                pli = (PerLengthSequenceImpedance)perLengthImpedanceContainer[configuration.Name];
                return pli;
            }


            

            return pli;

        }

        private void addComponentToGlobalDictionary(IdentifiedObject component, Type type)
        {
            if (!globalComponentDictionary.ContainsKey(type))
            {
                globalComponentDictionary.Add(type, new Dictionary<string, IdentifiedObject>());
            }


            if (type.Equals(typeof(ACLineSegment)))
            {
                ACLineSegment acl = (ACLineSegment)component;

                globalComponentDictionary[type].Add(acl.MRID, acl);
            }
            else if (type.Equals(typeof(Terminal)))
            {
                Terminal tp = (Terminal)component;

                globalComponentDictionary[tp.GetType()].Add(tp.MRID, tp);

            }
            else if (type.Equals(typeof(Circuit)))
            {
                Circuit cr = (Circuit)component;

                globalComponentDictionary[cr.GetType()].Add(cr.MRID, cr);
            }
            else if (type.Equals(typeof(WireInfo)))
            {
                WireInfo wi = (WireInfo)component;
                globalComponentDictionary[wi.GetType()].Add(wi.MRID, wi);

            }
            else if (type.Equals(typeof(PerLengthSequenceImpedance)))
            {
                PerLengthSequenceImpedance pli = (PerLengthSequenceImpedance)component;
                globalComponentDictionary[pli.GetType()].Add(pli.MRID, pli);
            }
            else if (type.Equals(typeof(ConnectivityNode)))
            {
                ConnectivityNode cn = (ConnectivityNode)component;
                globalComponentDictionary[cn.GetType()].Add(cn.MRID, cn);
            }
            else if (type.Equals(typeof(PowerTransformer)))
            {
                PowerTransformer pt = (PowerTransformer)component;
                globalComponentDictionary[pt.GetType()].Add(pt.MRID, pt);
            }
            else if (type.Equals(typeof(PowerTransformerEnd)))
            {
                PowerTransformerEnd pt = (PowerTransformerEnd)component;
                globalComponentDictionary[pt.GetType()].Add(pt.MRID, pt);
            }
            else if (type.Equals(typeof(UsagePoint)))
            {
                UsagePoint us = (UsagePoint)component;
                globalComponentDictionary[us.GetType()].Add(us.MRID, us);
            }
            else if (type.Equals(typeof(PSRType)))
            {
                PSRType psr = (PSRType)component;
                globalComponentDictionary[psr.GetType()].Add(psr.Name, psr);
            }
            else if (type.Equals(typeof(SynchronousMachine)))
            {
                SynchronousMachine sm = (SynchronousMachine)component;
                globalComponentDictionary[sm.GetType()].Add(sm.Name, sm);
            }

        }


        private double feetsToMeters(double length)
        {
            return (length / 3.2808);
        }
    }

}