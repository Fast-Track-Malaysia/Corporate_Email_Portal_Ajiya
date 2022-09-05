using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Security;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraReports.UI;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.XtraReports.Parameters;
using System.Reflection;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.ReportsV2;


namespace Web
{
    public class XafReport
    {
        public XPObjectSpaceProvider objectSpaceProvider { get; set; }
        public XafReport(string xafconnectionstring, string xafAssemblyName)
        {
            //FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;

            XpoTypesInfoHelper.ForceInitialize();
            ITypesInfo typesInfo = XpoTypesInfoHelper.GetTypesInfo();
            XpoTypeInfoSource xpoTypeInfoSource = XpoTypesInfoHelper.GetXpoTypeInfoSource();
            typesInfo.RegisterEntity(typeof(ReportDataV2));

            string[] assemstring = xafAssemblyName.Split(',');
            Assembly a = null;
            foreach (string x in assemstring)
            {
                a = Assembly.Load(x);

                Type[] t = a.GetTypes();
                try
                {
                    foreach (Type type in t)
                    {
                        if (typeof(XPObject).IsAssignableFrom(type))
                            typesInfo.RegisterEntity(type);
                        if (typeof(XPLiteObject).IsAssignableFrom(type))
                            typesInfo.RegisterEntity(type);
                        if (typeof(BaseObject).IsAssignableFrom(type))
                            typesInfo.RegisterEntity(type);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            if (a == null)
            {
                throw new Exception($"xaf assembly {xafAssemblyName} cannot load");
            }

            ConnectionStringDataStoreProvider dataStoreProvider = new ConnectionStringDataStoreProvider(xafconnectionstring);
            objectSpaceProvider = new XPObjectSpaceProvider(dataStoreProvider, typesInfo, xpoTypeInfoSource);
        }

    }
    public class MyReportDataSourceHelper : ReportDataSourceHelper
    {
        IObjectSpaceProvider objectSpaceProvider;
        public MyReportDataSourceHelper(IObjectSpaceProvider objectSpaceProvider) : base(null)
        {
            this.objectSpaceProvider = objectSpaceProvider;
        }
        protected override IReportObjectSpaceProvider CreateReportObjectSpaceProvider()
        {
            return new MyReportObjectSpaceProvider(objectSpaceProvider);
        }
    }
    public class MyReportObjectSpaceProvider : IReportObjectSpaceProvider, IObjectSpaceCreator
    {
        IObjectSpaceProvider objectSpaceProvider;
        IObjectSpace objectSpace;
        public MyReportObjectSpaceProvider(IObjectSpaceProvider objectSpaceProvider)
        {
            this.objectSpaceProvider = objectSpaceProvider;
        }
        public void DisposeObjectSpaces()
        {
            if (objectSpace != null)
            {
                objectSpace.Dispose();
                objectSpace = null;
            }
        }
        public IObjectSpace GetObjectSpace(Type type)
        {
            if (objectSpace == null)
            {
                objectSpace = objectSpaceProvider.CreateObjectSpace();
            }
            return objectSpace;
        }
        public IObjectSpace CreateObjectSpace(Type type)
        {
            return objectSpaceProvider.CreateObjectSpace();
        }
    }
}