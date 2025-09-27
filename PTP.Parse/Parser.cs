using PTP.Parse.Data;
using PTP.Parse.Interfaces;
using PTP.Query;
using PTP.Shared;

namespace PTP.Parse
{
    public class Parser : IParser
    {
        public ILexer Lexer { get; set; }

        public Parser(string text)
        {
            Lexer = new Lexer(text);
        }

        //These are methods for predicate parsing

        public string Field()
        {
            return Lexer.EatId();
        }

        public Constant Constant()
        {
            if (Lexer.MatchStringConstant())
                return new Constant(Lexer.EatStringConstant());
            else
                return new Constant(Lexer.EatIntConstant());
        }

        public Expression Expression()
        {
            if(Lexer.MatchId())
                return new Expression(this.Field());
            else
                return new Expression(Constant());
        }

        public Term Term()
        {
            Expression lhs = Expression();
            Lexer.EatDelim('=');
            Expression rhs = Expression();

            return new Term(lhs, rhs);
        }

        public Predicate Predicate()
        {
            Predicate pred = new Predicate(Term());
            if (Lexer.MatchKeyword("and"))
            {
                Lexer.EatKeyword("and");
                pred.ConjoinWith(this.Predicate());
            }

            return pred;
        }

        //These are methods for query parsing

        public QueryData Query()
        {
            Lexer.EatKeyword("select");
            List<string> fields = this.SelectList();

            Lexer.EatKeyword("from");

            ICollection<string> tables = this.TableList();

            Predicate pred = new Predicate();

            if(Lexer.MatchKeyword("where"))
            {
                Lexer.EatKeyword("where");
                pred = this.Predicate();
            }

            return new QueryData(fields, tables, pred);
        }

        private ICollection<string> TableList()
        {
            List<string> tables = new List<string>();
            tables.Add(Lexer.EatId());
            if (Lexer.MatchDelim(','))
            {
                Lexer.EatDelim(',');
                tables.AddRange(this.TableList());
            }
            return tables;
        }

        private List<string> SelectList()
        {
            List<string> fields = new List<string>();
            fields.Add(Lexer.EatId());
            if (Lexer.MatchDelim(','))
            {
                Lexer.EatDelim(',');
                fields.AddRange(this.SelectList());
            }

            return fields;
        }

        //These are methods for parsing update commands

        public object UpdateCommand()
        {
            if (Lexer.MatchKeyword("insert"))
                return this.Insert();
            else if (Lexer.MatchKeyword("delete"))
                return this.Delete();
            else if (Lexer.MatchKeyword("update"))
                return this.Modify();
            else
                return this.CreateCommand();
        }

        //These are methods for parsing modify commands

        private ModifyData Modify()
        {
            Lexer.EatKeyword("update");
            string tableName = Lexer.EatId();
            Lexer.EatKeyword("set");
            string fieldName = this.Field();
            Lexer.EatDelim('=');
            Expression newValue = this.Expression();
            Predicate pred = new Predicate();
            if (Lexer.MatchKeyword("where"))
            {
                Lexer.EatKeyword("where");
                pred = this.Predicate();
            }
            return new ModifyData(tableName, fieldName, newValue, pred);
        }

        //These are methods for parsing delete commands

        private DeleteData Delete()
        {
            Lexer.EatKeyword("delete");
            Lexer.EatKeyword("from");
            string tableName = Lexer.EatId();

            Predicate pred = new Predicate();
            if (Lexer.MatchKeyword("where"))
            {
                Lexer.EatKeyword("where");
                pred = this.Predicate();
            }

            return new DeleteData(tableName, pred);
        }

        //These are for insert command parsing

        private InsertData Insert()
        {
            Lexer.EatKeyword("insert");
            Lexer.EatKeyword("into");
            string tableName = Lexer.EatId();
            Lexer.EatDelim('(');
            List<string> fields = this.FieldList();
            Lexer.EatDelim(')');
            Lexer.EatKeyword("values");
            Lexer.EatDelim('(');
            List<Constant> values = this.ConstantList();
            Lexer.EatDelim(')');

            return new InsertData(tableName, fields, values);
        }

        private List<Constant> ConstantList()
        {
            List<Constant> values = new List<Constant>();
            values.Add(this.Constant());
            if (Lexer.MatchDelim(','))
            {
                Lexer.EatDelim(',');
                values.AddRange(this.ConstantList());
            }
            return values;
        }

        private List<string> FieldList()
        {
            List<string> fields = new List<string>();
            fields.Add(this.Field());
            if (Lexer.MatchDelim(','))
            {
                Lexer.EatDelim(',');
                fields.AddRange(this.FieldList());
            }
            return fields;
        }

        //ese are methods for parsing create commands

        private object CreateCommand()
        {
            Lexer.EatKeyword("create");
            if (Lexer.MatchKeyword("table"))
                return this.CreateTable();
            else if (Lexer.MatchKeyword("view"))
                return this.CreateView();
            else
                return this.CreateIndex();
        }
        private CreateViewData CreateView()
        {
            Lexer.EatKeyword("view");
            string viewName = Lexer.EatId();
            Lexer.EatKeyword("as");
            QueryData queryData = this.Query();
            return new CreateViewData(viewName, queryData);
        }

        private CreateIndexData CreateIndex()
        {
            Lexer.EatKeyword("index");
            string indexName = Lexer.EatId();
            Lexer.EatKeyword("on");
            string tableName = Lexer.EatId();
            Lexer.EatDelim('(');
            string fieldName = this.Field();
            Lexer.EatDelim(')');

            return new CreateIndexData(indexName, tableName, fieldName);
        }

        // These are methods for parsing create table commands

        private CreateTableData CreateTable()
        {
            Lexer.EatKeyword("table");
            string tableName = Lexer.EatId();
            Lexer.EatDelim('(');
            Schema schema = this.FieldDefs();
            Lexer.EatDelim(')');

            return new CreateTableData(tableName, schema);
        }

        private Schema FieldDefs()
        {
            Schema schema = this.FieldDef();

            if(Lexer.MatchDelim(','))
            {
                Lexer.EatDelim(',');
                schema.AddAll(this.FieldDefs());
            }

            return schema;
        }

        private Schema FieldDef()
        {
            string fieldName = this.Field();

            Schema schema = new Schema();

            if (Lexer.MatchKeyword("int"))
            {
                Lexer.EatKeyword("int");
                schema.AddIntField(fieldName);
            }
            else
            {
                Lexer.EatKeyword("varchar");
                Lexer.EatDelim('(');
                int length = Lexer.EatIntConstant();
                Lexer.EatDelim(')');

                schema.AddStringField(fieldName, length);
            }

            return schema;
        }
    }
}
