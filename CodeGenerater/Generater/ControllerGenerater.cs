using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
    /// <summary>
    /// Java Controller Generater
    /// </summary>
    class ControllerGenerater : BaseGenerater
    {       
        public ControllerGenerater(LocalSchema schema, Connection connection)
        {
            mDbLocalSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = StringHelper.dbNameToClassName(StringHelper.DBNamingToCamelCase(mTableName));
        }

        public String GeneraterCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"package {mConnection.pakeage}.controller;");
            sb.AppendLine();
            sb.AppendLine("import org.springframework.web.bind.annotation.*;");
            sb.AppendLine("import org.springframework.beans.factory.annotation.Autowired;");
            sb.AppendLine();
            sb.AppendLine("import java.util.Date;");
            sb.AppendLine("import java.util.List;");
            sb.AppendLine("import java.util.stream.Collectors;");

            sb.AppendLine();
            sb.AppendLine("@RestController");
            sb.AppendLine("@RequestMapping(\" 访问路径 \")");
            sb.AppendLine($"public class {ClassNmae}Controller "+"{");
            sb.AppendLine();

            sb.AppendLine(tab+$"@Autowired");
            sb.AppendLine(tab + $"{ClassNmae}Service {StringHelper.LowerCaseFirstLetter(ClassNmae)}service;");
            sb.AppendLine();

            sb.AppendLine(tab + $"// 控制器业务不好确定。只生成一个类文件结构，不带任何方法");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("}");
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
