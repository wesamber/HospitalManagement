using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Configuration;

public class FileStorageOption
{
    // نفس الاسم يلي رح نستخدمه في appsettings.json
    public const string SectionName = "StorageSettings";
    public string ActiveProvider { get; set; } = string.Empty;
    public JsonPathsOptions JsonPaths { get; set; } = new ();
    public ConnectionStringsOptions ConnectionStrings { get; set; } = new ();
}
