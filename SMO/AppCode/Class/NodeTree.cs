namespace SMO
{
    public class Node
    {
        public string id { get; set; }
        public string pId { get; set; }
        public string name { get; set; }
        public string @checked { get; set; }
        public string open { get; set; }
        public string icon { get; set; }
        public string nocheck { get; set; }
        public string isParent { get; set; }

        public Node()
        {
            @checked = "false";
            open = "false";
        }
    }

    public class NodeOrganize : Node
    {
        public string type { get; set; }
        public NodeOrganize() : base()
        {

        }
    }

    public class NodeUser : Node
    {
        public string userName { get; set; }
        public string type { get; set; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public NodeUser() : base()
        {

        }
    }

    public class NodeSpec : Node
    {
        public string type { get; set; }
        public NodeSpec() : base()
        {

        }
    }

    public class NodeConfig : Node
    {
        public string companyCode { get; set; }
        public string modulType { get; set; }
        public NodeConfig() : base()
        {

        }
    }

    public class NodeRight : Node
    {
        public string isAdd { get; set; }
        public string isRemove { get; set; }
        public NodeRight() : base()
        {

        }
    }
    public class NodeMenu : Node
    {
        public NodeMenu() : base()
        {

        }
    }

    public class NodeWorkFlow : Node
    {
        public string type { get; set; }
        public NodeWorkFlow() : base()
        {

        }
    }

    public class NodeCostCenter : Node
    {
        public string type { get; set; }
        public bool chkDisabled { get; set; }
        public NodeCostCenter() : base()
        {

        }
    }

    public class NodeCompany : Node
    {
        public bool chkDisabled;
        public string type { get; set; }
        public NodeCompany() : base()
        {

        }
    }
    public class NodeProject : Node
    {
        public bool chkDisabled { get; set; }
        public string type { get; set; }
        public NodeProject() : base()
        {

        }
    }


    public class NodeInternalOrder : Node
    {
        public string type { get; set; }
        public bool chkDisabled { get; set; }
        public NodeInternalOrder() : base()
        {

        }
    }

    public class NodeTableau : Node
    {
        public string isGroup { get; set; }
        public NodeTableau() : base()
        {

        }
    }

    public class NodeCostElement : Node
    {
        public string isGroup { get; set; }
        public string isSap { get; set; }
        public string type { get; set; }
        public NodeCostElement() : base()
        {

        }
    }

    public class NodeRevenueElement : Node
    {
        public string isGroup { get; set; }
        public string isSap { get; set; }
        public string type { get; set; }
        public NodeRevenueElement() : base()
        {

        }
    }

    public class NodeProfitCenter : Node
    {
        public string type { get; set; }
        public bool chkDisabled { get; set; }

        public NodeProfitCenter() : base()
        {

        }
    }

    public class NodeDataFlow : Node
    {
        public NodeDataFlow() : base()
        {
        }

        public int? version { get; set; }
        public int year { get; set; }
        public string realId { get; set; }
        public int sumUpVersion { get; internal set; }
    }

}