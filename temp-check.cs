using System;
using System.Linq;
using System.Reflection;
using Microsoft.Kiota.Abstractions;
Console.WriteLine(typeof(IRequestAdapter).FullName);
foreach (var m in typeof(IRequestAdapter).GetMethods()) Console.WriteLine(m.ToString());
