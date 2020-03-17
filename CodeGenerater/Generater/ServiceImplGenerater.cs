using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace CodeGenerater
{
    public class ServiceImplGenerater : BaseGenerater
    {
        private String mapper = "";

        public ServiceImplGenerater(LocalSchema schema, Connection connection)
        {
            mDbLocalSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = StringHelper.dbNameToClassName(StringHelper.DBNamingToCamelCase(mTableName));
        }

        public String GeneraterCode()
        {
            StringBuilder sb = new StringBuilder();
            String pakage = $"package {mConnection.pakeage}.service.impl;";
            sb.AppendLine(pakage);
            sb.AppendLine();
            sb.AppendLine("import java.util.List;");
            sb.AppendLine($"import {mConnection.pakeage}.{ClassNmae};");
            sb.AppendLine($"import {mConnection.pakeage}.mapper.{ClassNmae}Mapper;");
            sb.AppendLine($"import org.springframework.beans.factory.annotation.Autowired;");
            sb.AppendLine($"import org.springframework.stereotype.Service;");
            sb.AppendLine($"import org.springframework.transaction.annotation.Transactional;");
            sb.AppendLine();
            sb.AppendLine(GetClassComment());
            sb.AppendLine("Service(\"CategoryService\")");
            sb.AppendLine($"public class {ClassNmae}ServiceImpl implements {ClassNmae}Service " + "{");
            sb.AppendLine();

            sb.AppendLine(tab + "@Autowired");
            mapper = StringHelper.LowerCaseFirstLetter(ClassNmae)+ "Mapper";
            sb.AppendLine(tab + $"{ClassNmae}Mapper {mapper};");
            sb.AppendLine();
            sb.AppendLine(tab + "@Override");
            sb.AppendLine(tab + $"public int deleteByPrimaryKey(Integer id)");
            sb.AppendLine(tab + "{");
            sb.AppendLine(tab + tab + $"return {mapper}.deleteByPrimaryKey(id);");            
            sb.AppendLine(tab + "}");
            sb.AppendLine();
            sb.AppendLine(tab + "@Override");
            sb.AppendLine(tab + $"public int insert({ClassNmae} record)");
            sb.AppendLine(tab + "{");
            sb.AppendLine(tab + tab + $"return {mapper}.insert(record);");
            sb.AppendLine(tab + "}");
            sb.AppendLine();
            sb.AppendLine(tab + "@Override");
            sb.AppendLine(tab + $"public int insertSelective({ClassNmae} record)");
            sb.AppendLine(tab + "{");
            sb.AppendLine(tab + tab + $"return {mapper}.insertSelective(record);");
            sb.AppendLine(tab + "}");
            sb.AppendLine();
            sb.AppendLine(tab + "@Override");
            sb.AppendLine(tab + $" public {ClassNmae} selectByPrimaryKey(Integer id)");
            sb.AppendLine(tab + "{");
            sb.AppendLine(tab + tab + $"return {mapper}.selectByPrimaryKey(id);");
            sb.AppendLine(tab + "}");
            sb.AppendLine();
            sb.AppendLine(tab + "@Override");
            sb.AppendLine(tab + $" public int updateByPrimaryKeySelective({ClassNmae} record)");
            sb.AppendLine(tab + "{");
            sb.AppendLine(tab + tab + $"return {mapper}.updateByPrimaryKeySelective(record);");
            sb.AppendLine(tab + "}");
            sb.AppendLine();
            sb.AppendLine(tab + "@Override");
            sb.AppendLine(tab + $" public int updateByPrimaryKey({ClassNmae} record)");
            sb.AppendLine(tab + "{");
            sb.AppendLine(tab + tab + $"return {mapper}.updateByPrimaryKey(record);");
            sb.AppendLine(tab + "}");
            sb.AppendLine();
            sb.AppendLine(tab + "@Override");
            sb.AppendLine(tab + $" public List<{ClassNmae}> getList() ");
            sb.AppendLine(tab + "{");
            sb.AppendLine(tab + tab + $"throw new  NotImplementedException(); ");
            sb.AppendLine(tab+"}");
            sb.AppendLine("}");
            return sb.ToString();
        }

    }
}
