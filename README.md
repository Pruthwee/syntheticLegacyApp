[TRACEABILITY_MATRIX.md](https://github.com/user-attachments/files/26322069/TRACEABILITY_MATRIX.md)
# Synthetic Legacy App — Traceability Matrix
## Concierto Modernize · M-Basic (Cloud Readiness) Rule Coverage

**Total Rules Covered:** 65  
**Total Source Files:** 65  
**Coverage:** 1 file per rule (1:1 traceability)

> Each file contains inline `// VIOLATION <ruleId>:` comments at every
> violation site for precise scanner alignment.

TESTERS: Please refer to Violation_Traceability_Matrix.csv for source file name, line number, rule name and rule description traceability.
---

## Summary by Folder

| Folder            | Files | Rule IDs Covered |
|-------------------|------:|------------------|
| Authentication    |     5 | 0029, 0030, 0031, 0032, 0055 |
| Configuration     |     6 | 0009, 0010, 0011, 0012, 0017, 0123 |
| Database          |     4 | 0013, 0014, 0015, 0016 |
| ExternalServices  |     1 | 0056 |
| FileSystem        |     5 | 0001, 0002, 0003, 0004, 0054 |
| LegacyFramework   |    10 | 0025, 0026, 0028, 0047, 0048, 0049, 0057, 0058, 0059, 0060 |
| Logging           |     3 | 0033, 0034, 0035 |
| Memory            |     2 | 0036, 0037 |
| Networking        |     5 | 0018, 0019, 0020, 0027, 0125 |
| Platform          |     9 | 0040, 0041, 0042, 0043, 0044, 0050, 0051, 0052, 0053 |
| Runtime           |     3 | 0121, 0126, 0127 |
| State             |     6 | 0005, 0006, 0007, 0008, 0045, 0046 |
| Threading         |     6 | 0021, 0022, 0023, 0024, 0039, 0124 |
| **Total**         | **65** | |

---

## Full Rule-to-File Mapping

| Rule ID          | Rule Name                            | Category        | File Path                                              | Primary Violation Pattern |
|------------------|--------------------------------------|-----------------|--------------------------------------------------------|---------------------------|
| cr-dotnet-0001   | Hard-coded File Paths                | FileSystem      | FileSystem/HardCodedFilePaths.cs                       | `@"C:\Reports\Output"` absolute Windows path constants |
| cr-dotnet-0002   | Local File System Write Operations   | FileSystem      | FileSystem/LocalFileSystemWrite.cs                     | `File.WriteAllText`, `FileStream` writes to local paths |
| cr-dotnet-0003   | System.IO.File for Data Storage      | FileSystem      | FileSystem/SystemIOFileStorage.cs                      | `File.ReadAllBytes`, `File.WriteAllBytes` as persistence layer |
| cr-dotnet-0004   | Directory.GetFiles Usage             | FileSystem      | FileSystem/DirectoryGetFilesUsage.cs                   | `Directory.GetFiles`, `Directory.EnumerateFiles` |
| cr-dotnet-0005   | IIS Application State                | State           | State/IISApplicationState.cs                           | `HttpApplication.Application[key]` read/write |
| cr-dotnet-0006   | Static Collections for State         | State           | State/StaticCollectionsState.cs                        | `static Dictionary<>`, `static List<>` mutable fields |
| cr-dotnet-0007   | MemoryCache Without Expiration       | State           | State/MemoryCacheNoExpiry.cs                           | `MemoryCache.Set` with no `CacheItemPolicy.AbsoluteExpiration` |
| cr-dotnet-0008   | Singleton Pattern with State         | State           | State/SingletonWithState.cs                            | `static readonly` singleton with mutable state fields |
| cr-dotnet-0009   | Hard-coded Connection Strings        | Configuration   | Configuration/HardCodedConnectionStrings.cs            | Connection string literals embedded in `const`/code |
| cr-dotnet-0010   | Web.config Transformations           | Configuration   | Configuration/WebConfigTransformations.cs              | `XmlDocument` loading `Web.{env}.config` at runtime |
| cr-dotnet-0011   | Hard-coded Service URLs              | Configuration   | Configuration/HardCodedServiceUrls.cs                  | `const string BaseUrl = "http://prod.internal..."` |
| cr-dotnet-0012   | Machine.config Dependencies          | Configuration   | Configuration/MachineConfigDeps.cs                     | `ConfigurationManager` reading machine-wide config keys |
| cr-dotnet-0013   | SqlConnection Direct Usage           | Database        | Database/SqlConnectionDirect.cs                        | `new SqlConnection(...)` without pooling framework |
| cr-dotnet-0014   | SQL Server Specific Features         | Database        | Database/SqlServerSpecific.cs                          | `NOLOCK`, `FOR XML`, `NEWID()`, SQL Server-only T-SQL |
| cr-dotnet-0015   | SqlDependency Usage                  | Database        | Database/SqlDependencyUsage.cs                         | `SqlDependency.Start`, `cmd.Notification` subscription |
| cr-dotnet-0016   | TransactionScope with DTC            | Database        | Database/TransactionScopeWithDTC.cs                    | `TransactionScope` spanning multiple connections (DTC) |
| cr-dotnet-0017   | Hard-coded Port Numbers              | Configuration   | Configuration/HardCodedPortNumbers.cs                  | `const int SqlPort = 1433`, `RedisPort = 6379` literals |
| cr-dotnet-0018   | .NET Remoting Usage                  | Networking      | Networking/DotNetRemoting.cs                           | `MarshalByRefObject`, `RemotingConfiguration.Configure` |
| cr-dotnet-0019   | WCF NetTcpBinding                    | Networking      | Networking/WCFNetTcpBinding.cs                         | `new NetTcpBinding()` with `ChannelFactory<T>` |
| cr-dotnet-0020   | UDP Socket Programming               | Networking      | Networking/UDPSocketProgramming.cs                     | `UdpClient.Send`, `Socket(SocketType.Dgram, ProtocolType.Udp)` |
| cr-dotnet-0021   | Thread.Start() Usage                 | Threading       | Threading/ThreadStartUsage.cs                          | `new Thread(...).Start()` unbounded thread creation |
| cr-dotnet-0022   | Hard-coded ThreadPool Size           | Threading       | Threading/HardCodedThreadPool.cs                       | `ThreadPool.SetMinThreads(50, 50)` hard-coded values |
| cr-dotnet-0023   | ThreadStatic Attribute               | Threading       | Threading/ThreadStaticAttribute.cs                     | `[ThreadStatic] static` field declarations |
| cr-dotnet-0024   | Timer Without Synchronization        | Threading       | Threading/TimerNoSync.cs                               | `System.Threading.Timer` with no cross-instance lock |
| cr-dotnet-0025   | .NET Framework < 4.6.1               | LegacyFramework | LegacyFramework/DotNetFrameworkVersion.cs              | `[TargetFramework(".NETFramework,Version=v4.5")]`, `SecurityProtocolType.Ssl3` |
| cr-dotnet-0026   | Web Forms Usage                      | LegacyFramework | LegacyFramework/WebFormsUsage.cs                       | `: Page`, `ViewState`, `IsPostBack`, `Response.Redirect` |
| cr-dotnet-0027   | WCF Service Host                     | Networking      | Networking/WCFServiceHost.cs                           | `new ServiceHost(typeof(...))`, `host.Open()` |
| cr-dotnet-0028   | Enterprise Library Usage             | LegacyFramework | LegacyFramework/EnterpriseLibrary.cs                   | `DatabaseFactory.CreateDatabase`, `Logger.Write(entry)` |
| cr-dotnet-0029   | Forms Authentication                 | Authentication  | Authentication/FormsAuthentication.cs                  | `FormsAuthentication.SetAuthCookie`, `.RedirectFromLoginPage` |
| cr-dotnet-0030   | Windows Authentication               | Authentication  | Authentication/WindowsAuthentication.cs                | `WindowsIdentity.GetCurrent()`, NTLM/Kerberos token use |
| cr-dotnet-0031   | IP Address Restrictions              | Authentication  | Authentication/IPAddressRestrictions.cs                | `IPAddress.Parse` allowlist logic on `Request.UserHostAddress` |
| cr-dotnet-0032   | Certificate Store Access             | Authentication  | Authentication/CertificateStoreAccess.cs               | `new X509Store(StoreName.My, StoreLocation.LocalMachine)` |
| cr-dotnet-0033   | EventLog Writing                     | Logging         | Logging/EventLogWriting.cs                             | `new EventLog("Application")`, `EventLog.WriteEntry` |
| cr-dotnet-0034   | TraceListener File Output            | Logging         | Logging/TraceListenerFileOutput.cs                     | `new TextWriterTraceListener(@"C:\Logs\...")` |
| cr-dotnet-0035   | Custom Log4Net Appenders             | Logging         | Logging/Log4NetFileAppender.cs                         | `RollingFileAppender` writing to local file path |
| cr-dotnet-0036   | Large Object Heap Issues             | Memory          | Memory/LargeObjectHeap.cs                              | `new byte[1024 * 1024]`, `File.ReadAllBytes` into single array |
| cr-dotnet-0037   | Synchronous HttpClient               | Memory          | Memory/SynchronousHttpClient.cs                        | `GetAsync().Result`, `PostAsync().Wait()`, `GetAwaiter().GetResult()` |
| cr-dotnet-0039   | Blocking Collection Operations       | Threading       | Threading/BlockingCollectionOps.cs                     | `BlockingCollection<T>.Take()` without timeout/cancellation |
| cr-dotnet-0040   | Registry Access                      | Platform        | Platform/RegistryAccess.cs                             | `Registry.LocalMachine.OpenSubKey(...)`, `RegistryKey.SetValue` |
| cr-dotnet-0041   | COM Interop Usage                    | Platform        | Platform/COMInterop.cs                                 | `[ComImport]`, `Type.GetTypeFromProgID`, `Marshal.ReleaseComObject` |
| cr-dotnet-0042   | P/Invoke Windows APIs                | Platform        | Platform/PInvokeWindowsAPIs.cs                         | `[DllImport("kernel32.dll")]`, `[DllImport("user32.dll")]` |
| cr-dotnet-0043   | Message Queue                        | Platform        | Platform/MSMQMessageQueue.cs                           | `new MessageQueue(@".\private$\orders")`, `queue.Send` |
| cr-dotnet-0044   | IIS Module Dependencies              | Platform        | Platform/IISModuleDeps.cs                              | `: IHttpModule`, `: IHttpHandler`, `HttpContext.Current` |
| cr-dotnet-0045   | Session State Provider               | State           | State/SessionStateInProc.cs                            | `HttpSessionState` InProc — `Session["key"] = value` |
| cr-dotnet-0046   | Output Cache Provider                | State           | State/OutputCacheInMemory.cs                           | `[OutputCache(Duration=300)]`, `OutputCache.Insert` |
| cr-dotnet-0047   | MSBuild Custom Tasks                 | LegacyFramework | LegacyFramework/MSBuildCustomTasks.cs                  | `: Task`, `override bool Execute()`, `msdeploy.exe` invocation |
| cr-dotnet-0048   | ClickOnce Deployment                 | LegacyFramework | LegacyFramework/ClickOnceDeployment.cs                 | `ApplicationDeployment.IsNetworkDeployed`, `.CheckForDetailedUpdate()` |
| cr-dotnet-0049   | Assembly GAC References              | LegacyFramework | LegacyFramework/AssemblyGACRefs.cs                     | `Assembly.Load("..., PublicKeyToken=...")` strong-name GAC load |
| cr-dotnet-0050   | Performance Counters                 | Platform        | Platform/PerformanceCounters.cs                        | `new PerformanceCounter(...)`, `PerformanceCounterCategory.Create` |
| cr-dotnet-0051   | Named Pipes Local                    | Platform        | Platform/NamedPipesLocal.cs                            | `new NamedPipeServerStream(...)`, `new NamedPipeClientStream(...)` |
| cr-dotnet-0052   | Mutex Machine-Wide                   | Platform        | Platform/MutexMachineWide.cs                           | `new Mutex(false, @"Global\LegacyApp_...")` named global mutex |
| cr-dotnet-0053   | Environment.MachineName              | Platform        | Platform/EnvironmentMachineName.cs                     | `Environment.MachineName` in business logic / audit fields |
| cr-dotnet-0054   | Hard-coded Temp Paths                | FileSystem      | FileSystem/HardCodedTempPaths.cs                       | `@"C:\Temp\"`, `@"C:\Windows\Temp\"` hard-coded temp references |
| cr-dotnet-0055   | ActiveDirectory Dependencies         | Authentication  | Authentication/ActiveDirectoryDeps.cs                  | `new DirectoryEntry("LDAP://...")`, `DirectorySearcher` |
| cr-dotnet-0056   | SMTP Local Server                    | ExternalServices| ExternalServices/SMTPLocalServer.cs                    | `new SmtpClient("localhost", 25)`, `SmtpDeliveryMethod.PickupDirectoryFromIis` |
| cr-dotnet-0057   | Crystal Reports Usage                | LegacyFramework | LegacyFramework/CrystalReports.cs                      | `new ReportDocument()`, `ExportToStream(ExportFormatType.PortableDocFormat)` |
| cr-dotnet-0058   | SharePoint Dependencies              | LegacyFramework | LegacyFramework/SharePointDependencies.cs              | `new SPSite(url)`, `SPList`, `SPWorkflowManager` |
| cr-dotnet-0059   | BizTalk Artifacts                    | LegacyFramework | LegacyFramework/BizTalkArtifacts.cs                    | `: IBaseComponent`, `: IComponent`, `XLANGMessage` parameter |
| cr-dotnet-0060   | Windows Service Base                 | LegacyFramework | LegacyFramework/WindowsServiceBase.cs                  | `: ServiceBase`, `OnStart`, `OnStop`, `ServiceBase.Run(...)` |
| cr-dotnet-0121   | Clock/Time Dependencies              | Runtime         | Runtime/ClockTimeDependencies.cs                       | `DateTime.Now`, `TimeZone.CurrentTimeZone`, `DateTime.Today` |
| cr-dotnet-0123   | Lack of Externalized Secrets         | Configuration   | Configuration/HardCodedSecrets.cs                      | API keys, passwords, tokens embedded as `const string` |
| cr-dotnet-0124   | Fixed Thread Affinity/Pinning        | Threading       | Threading/FixedThreadAffinity.cs                       | `Process.GetCurrentProcess().ProcessorAffinity = ...` |
| cr-dotnet-0125   | Unsupported Protocols                | Networking      | Networking/UnsupportedProtocols.cs                     | `FtpWebRequest`, UNC path `\\server\share`, SMB access |
| cr-dotnet-0126   | Heavy Coupling to Stateful Middleware| Runtime         | Runtime/StatefulMiddleware.cs                          | `IRequiresSessionState`, `InstanceContextMode.PerSession` WCF |
| cr-dotnet-0127   | Missing Graceful Shutdown Hooks      | Runtime         | Runtime/MissingShutdownHooks.cs                        | No `AppDomain.ProcessExit`, no `CancellationToken`, no `StopAsync` |

---

## Violation Comment Convention

Every source file uses standardised inline violation markers:

```
// VIOLATION cr-dotnet-XXXX: <brief explanation of what triggers the rule>
```

Scanners and reviewers can `grep -rn "VIOLATION cr-dotnet-"` across the
repository to enumerate all seeded violation sites.

---

*Generated for Concierto Modernize QA validation — Trianz internal use only.*
