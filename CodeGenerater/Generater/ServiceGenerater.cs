using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
    public class ServiceGenerater : BaseGenerater
    {
        public ServiceGenerater(LocalSchema schema, Connection connection)
        {
            mDbLocalSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = StringHelper.dbNameToClassName(StringHelper.DBNamingToCamelCase(mTableName));
        }

        public String GeneraterCode()
        {
            StringBuilder sb = new StringBuilder();
            String pakage = $"package {mConnection.pakeage}.service;";
            sb.AppendLine(pakage);
            sb.AppendLine();
            sb.AppendLine("import java.util.List;");
            sb.AppendLine($"import {mConnection.pakeage}.{ClassNmae};");
            sb.AppendLine();
            sb.AppendLine(GetClassComment());            
            sb.AppendLine($"public interface {ClassNmae}Service "+"{");
            sb.AppendLine();
            sb.AppendLine($"{tab} int deleteByPrimaryKey(Integer id);");
            sb.AppendLine();
            sb.AppendLine($"{tab} int insert({ClassNmae} record);");
            sb.AppendLine();
            sb.AppendLine($"{tab} int insertSelective({ClassNmae} record);");
            sb.AppendLine();
            sb.AppendLine($"{tab} {ClassNmae} selectByPrimaryKey(Integer id);");
            sb.AppendLine();
            sb.AppendLine($"{tab} int updateByPrimaryKeySelective({ClassNmae} record);");
            sb.AppendLine();
            sb.AppendLine($"{tab} int updateByPrimaryKey({ClassNmae} record);");
            sb.AppendLine();
            sb.AppendLine($"{tab} List<{ClassNmae}> getList();");
            sb.AppendLine();            
            sb.AppendLine("}");
            return sb.ToString();
        }

    }
}
