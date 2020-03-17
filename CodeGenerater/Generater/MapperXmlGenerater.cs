
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;

namespace CodeGenerater
{
    class MapperXmlGenerater : BaseGenerater
    {
        private Int32 wrapNumber = 5;//换行列数
        //以下属性在获取 BaseResultMap时赋值
        private String PrimaryKeyJdbcType = "INTEGER";//主键字段
        private String PrimaryColumnName = "id";  //表中的主键字段
        private String PrimaryKeyParameterType = "java.lang.Integer";//主键字段的Java 类型
        private Dictionary<String, String> fields = new Dictionary<String,String>();
        
        public MapperXmlGenerater(LocalSchema schema, Connection connection)
        {
            mDbLocalSchema = schema;
            mTableName = schema.TableName;
            mConnection = connection;
            ClassNmae = StringHelper.dbNameToClassName(StringHelper.DBNamingToCamelCase(mTableName));
        }

        public String GeneraterCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF - 8\"?>");
            sb.AppendLine("<!DOCTYPE mapper PUBLIC \" -//mybatis.org//DTD Mapper 3.0//EN\" \"http://mybatis.org/dtd/mybatis-3-mapper.dtd\">");
            sb.AppendLine($"<mapper namespace={mConnection.pakeage}.mapper.{ClassNmae}Mapper>");
            sb.AppendLine(GetResultMap());
            sb.AppendLine(GetBaseColumnList());
            sb.AppendLine(tab + $" < select id = \"selectByPrimaryKey\" parameterType = \"" + PrimaryKeyParameterType + "\" resultMap = \"BaseResultMap\" >");
            sb.AppendLine(tab + tab + " select < include refid = \"Base_Column_List\" />");
            sb.AppendLine(tab + tab + $"from {mDbLocalSchema.TableName}");
            sb.AppendLine(tab + tab + " where "+ PrimaryColumnName + " = #{id,jdbcType = " + fields[PrimaryColumnName] + "}");
            sb.AppendLine(tab + $"</select>");
            //deleteByPrimaryKey
            sb.AppendLine(tab + $" < delete id = \"deleteByPrimaryKey\" parameterType = \"java.lang.Integer\">");
            sb.AppendLine(tab + tab + $" delete from {mDbLocalSchema.TableName}");
            sb.AppendLine(tab + tab + " where "+PrimaryColumnName+" = #{id,jdbcType = " + fields[PrimaryColumnName] + "}");
            sb.AppendLine(tab + $"</delete>");
            //insert
            sb.AppendLine(tab + $"< insert id = \"insert\" parameterType = \"{mConnection.pakeage}.entity.{ClassNmae}\" >");
            var i = 0;
            sb.AppendLine(tab + $" insert into {mDbLocalSchema.TableName} (");
            string temp=String.Empty;
            foreach (var item in fields)
            {
               if(i % wrapNumber == 0)
                {
                    temp += String.IsNullOrEmpty(temp) == true ? "\r\n" + tab + tab + tab + item.Key : comma + "\r\n" + tab + tab + tab + item.Key;
                }
                else
                {
                    temp += String.IsNullOrEmpty(temp) == true ? item.Key : comma  + item.Key;
                }
                i++;
            }
            sb.AppendLine(tab + tab + temp);
            sb.AppendLine(tab + ")");
            sb.AppendLine(tab + "values (");
            i = 0;
            temp = string.Empty;
            foreach (var item in fields)
            {
                if (i % wrapNumber == 0 && i>0)
                {
                    temp += String.IsNullOrEmpty(temp) == true ? "\r\n"+tab+tab+tab+ item.Value : comma + "\r\n" + tab + tab + tab + item.Value;
                }
                else
                {
                    temp += String.IsNullOrEmpty(temp) == true ? item.Value : comma + item.Value;
                }
                i++;
            }
            sb.AppendLine(tab + tab + temp);
            sb.AppendLine(tab + ")");
            sb.AppendLine(tab+ " </insert>");
            //insertSelective
            sb.AppendLine(tab + $" < insert id = \"insertSelective\" parameterType = \"{mConnection.pakeage}.entity.{ClassNmae}\" useGeneratedKeys = \"true\" keyProperty = \""+PrimaryColumnName+"\" > ");
            sb.AppendLine(tab+tab+$" insert into {mDbLocalSchema.TableName}");
            sb.AppendLine(tab+tab+$"< trim prefix = \"(\" suffix = \")\" suffixOverrides = \", \" >");
            foreach (var item in fields)
            {
                sb.AppendLine(tab + tab +tab + $"<if test=\"{StringHelper.dbNameToClassName(item.Key)} != null\">{item.Key},</if> ");
            }
            sb.AppendLine(tab + tab + "</trim>");

            sb.AppendLine(tab + tab + $"<trim prefix=\"values(\" suffix=\")\" suffixOverrides=\", \">");
            foreach (var item in fields)
            {
                sb.AppendLine(tab + tab +tab + $"<if test=\"{StringHelper.dbNameToClassName(item.Key)} != null\">{item.Value},</if> ");
            }
            sb.AppendLine(tab + tab + "</trim>");
            sb.AppendLine(tab + " </insert>");
            //updateByPrimaryKeySelective
            sb.AppendLine(tab + $"< update id = \"updateByPrimaryKeySelective\" parameterType = \"{mConnection.pakeage}.entity.{ClassNmae}\" >");
            sb.AppendLine(tab +tab + $"update {mDbLocalSchema.TableName}");
            sb.AppendLine(tab + tab + tab + "  <set>");
            foreach (var item in fields)
            {
                sb.AppendLine(tab + tab + tab + tab + $" <if test=\"{StringHelper.dbNameToClassName(mDbLocalSchema.TableName)} != null\">{item.Key} = {item.Value}, </if>");
            }
            sb.AppendLine(tab + tab + tab + "  </set>");
            sb.AppendLine(tab +tab+ $" where {PrimaryColumnName} = {fields[PrimaryColumnName]}");
            sb.AppendLine(tab + " </update>");
            //updateByPrimaryKey
            sb.AppendLine(tab + $"< update id = \"updateByPrimaryKey\" parameterType = \"{mConnection.pakeage}.entity.{ClassNmae}\" >");
            sb.AppendLine(tab + tab + $"update {mDbLocalSchema.TableName} set ");
            foreach (var item in fields)
            {
                sb.AppendLine(tab + tab + tab  + $"{item.Key} ={item.Value} ,");
            }
            sb.AppendLine(tab + tab + $" where {PrimaryColumnName} = {fields[PrimaryColumnName]}");
            sb.AppendLine(tab + " </update>");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(tab + " <!--my-->");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("</mapper>");
            return sb.ToString();
        }

        private String GetResultMap()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + $"<resultMap id=\"BaseResultMap\" type={mConnection.pakeage}.entity.{ClassNmae}>");
            if (mConnection.type == DbType.mysql.ToString())
            {
                var datas = MySqlHelperInstance.GetTableSchema<MysqlTabeSchema>(mDbLocalSchema.TableName);
                foreach (var item in datas)
                {
                    string jdbcType = GetJdbcType(item.Type);
                    string property = GetProperty(item.Field);

                    if (item.Key == "PRI")
                    {
                        sb.AppendLine(tab + tab + $" <id column=\"{item.Field}\" jdbcType=\"{jdbcType}\" property=\"{property}\"/>");
                        PrimaryColumnName = item.Field;
                        PrimaryKeyJdbcType = GetJdbcType(item.Type);
                        PrimaryKeyParameterType = "java.lang." + JavaDbTypeMap.FindType(item.Type);
                        fields.Add(item.Field,"#{"+StringHelper.dbNameToClassName(item.Field)+ ",jdbcType="+GetJdbcType(item.Type)+"}");
                    }
                    else
                    {
                        sb.AppendLine(tab + tab + $" <result column=\"{item.Field}\" jdbcType=\"{jdbcType}\" property=\"{property}\"/>");
                        fields.Add(item.Field, "#{" + StringHelper.dbNameToClassName(item.Field) + ",jdbcType=" + GetJdbcType(item.Type) + "}");
                    }
                }
            }
            else
            {
                var datas = SQLiteHelperInstance.GetTableSchema<SqliteTableSchema>(mDbLocalSchema.TableName);
                foreach (var item in datas)
                {
                    string jdbcType = GetJdbcType(item.Type);
                    string property = GetProperty(item.Name);

                    if (item.Pk == "1")
                    {
                        sb.AppendLine(tab + tab + $" <id column=\"{item.Name}\" jdbcType=\"{jdbcType}\" property=\"{property}\"/>");
                    }
                    else
                    {
                        sb.AppendLine(tab + tab + $" <result column=\"{item.Name}\" jdbcType=\"{jdbcType}\" property=\"{property}\"/>");
                    }
                }
            }
            sb.AppendLine(tab + "</resultMap>");
            return sb.ToString();
        }

        private String GetBaseColumnList()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab + $"<sql id=\"Base_Column_List\">");
            String temp = String.Empty;
            List<MysqlTableColumnSchema> fields;
            if (mConnection.type == DbType.mysql.ToString())
            {
                fields = MySqlHelperInstance.GetTableColumnSchema(mConnection.dbName, mDbLocalSchema.TableName);
            }
            else
            {
                fields = SQLiteHelperInstance.GetTableColumnSchema(mConnection.dbName, mDbLocalSchema.TableName);
            }
            for (int i = 0; i < fields.Count; i++)
            {
                if (temp == String.Empty)
                {
                    temp = fields[i].ColumnName;
                }
                else
                {
                    if (i % wrapNumber == 0 && i>0)
                    {
                        temp += comma + "\r\n "+tab+tab+tab + fields[i].ColumnName;
                    }
                    else
                    {
                        temp += comma + " " + fields[i].ColumnName;
                    }
                }
            }
            sb.AppendLine(tab + temp);
            sb.AppendLine(tab + "</sql>");
            return sb.ToString();
        }



        private string GetJdbcType(string dbtype)
        {
            String s= "";
            if (dbtype.Contains("(")) {
                s = dbtype.Substring(0, dbtype.IndexOf("("));
            }            
            if (s.Contains("int"))
            {
                return "INTEGER";
            }
            return s.ToUpper();
        }
        private string GetProperty(string field)
        {
            return StringHelper.DBNamingToCamelCase(field);
        }
    }
}
