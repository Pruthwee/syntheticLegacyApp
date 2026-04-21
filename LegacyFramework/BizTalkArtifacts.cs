// =============================================================================
// RULE ID   : cr-dotnet-0059
// RULE NAME : BizTalk Artifacts
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application includes BizTalk Server components, orchestrations, or
//              pipeline artifacts that depend on BizTalk Server runtime. BizTalk
//              components are not cloud-compatible and require dedicated BizTalk
//              Server infrastructure.
// =============================================================================

using System;
using System.Xml;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace SyntheticLegacyApp.LegacyFramework
{
    // VIOLATION cr-dotnet-0059: Implements IBaseComponent — BizTalk pipeline component interface
    public class OrderValidationPipelineComponent : IBaseComponent, IComponent, IPersistPropertyBag
    {
        // VIOLATION cr-dotnet-0059: IBaseComponent properties — BizTalk pipeline metadata
        public string Name        => "OrderValidationPipelineComponent";
        public string Version     => "1.0.0";
        public string Description => "Validates inbound order XML messages in BizTalk receive pipeline.";

        private bool _strictValidation = true;

        // VIOLATION cr-dotnet-0059: IComponent.Execute — BizTalk pipeline stage execution
        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            // VIOLATION cr-dotnet-0059: IBaseMessagePart — BizTalk message part model
            IBaseMessagePart bodyPart = pInMsg.BodyPart;

            var doc = new XmlDocument();
            doc.Load(bodyPart.GetOriginalDataStream());

            XmlNode orderNode = doc.SelectSingleNode("//Order");
            if (orderNode == null)
            {
                Console.WriteLine("BizTalk: Missing Order element — message suspended.");
                throw new InvalidOperationException("Order element not found in BizTalk message.");
            }

            Console.WriteLine($"BizTalk pipeline validated order: {orderNode.InnerXml}");
            return pInMsg;
        }

        // VIOLATION cr-dotnet-0059: IPersistPropertyBag — BizTalk admin console property persistence
        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            object val = null;
            propertyBag.Read("StrictValidation", out val, 0);
            if (val != null) _strictValidation = (bool)val;
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            propertyBag.Write("StrictValidation", _strictValidation);
        }

        public void InitNew() { }
        public Guid ClassID => new Guid("A1B2C3D4-0001-0002-0003-A1B2C3D4E5F6");
    }

    // VIOLATION cr-dotnet-0059: Inherits XLANGMessage — BizTalk orchestration message type
    public class OrderOrchestrationHelper
    {
        // VIOLATION cr-dotnet-0059: XLANGMessage is the BizTalk orchestration message wrapper
        public void ProcessOrchestratedOrder(XLANGMessage orderMessage)
        {
            // VIOLATION cr-dotnet-0059: Reading part from BizTalk orchestration message
            XmlDocument doc = (XmlDocument)orderMessage["Body"].RetrieveAs(typeof(XmlDocument));
            Console.WriteLine($"Orchestration processing order: {doc.InnerXml}");
            orderMessage.Dispose();
        }

        // VIOLATION cr-dotnet-0059: BizTalk correlation set simulation — server-side only
        public void CorrelateResponse(string correlationId, string responseXml)
        {
            Console.WriteLine($"BizTalk correlation ID: {correlationId}");
            Console.WriteLine($"Correlating response to active orchestration instance.");
        }
    }
}
