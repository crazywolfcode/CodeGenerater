using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
  public  class MapperGenerater : BaseGenerater
    {
        public MapperGenerater(LocalSchema schema, Connection connection)
        {
            mDbLocalSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = StringHelper.dbNameToClassName(StringHelper.DBNamingToCamelCase(mTableName));
        }

        public String GeneraterCode()
        {
            StringBuilder sb = new StringBuilder();
            String pakage = $"package {mConnection.pakeage}.mapper;";
            sb.AppendLine(pakage);
            sb.AppendLine();
            sb.AppendLine("import java.util.List;");
            sb.AppendLine($"import {mConnection.pakeage}.{ClassNmae};");
            sb.AppendLine($"import org.apache.ibatis.annotations.Mapper;");
            sb.AppendLine($"import org.apache.ibatis.annotations.Param;");
            sb.AppendLine($"import org.springframework.stereotype.Component;");
            sb.AppendLine();          
            sb.AppendLine(GetCustomerClassComment());       
            sb.AppendLine("@Mapper");
            sb.AppendLine("@Component");
            sb.AppendLine($"public interface {ClassNmae}Service " + "{");
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
