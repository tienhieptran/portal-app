$(document).ready(
    function () {

        $(".tree-checkbox-hierarchical").fancytree({
            checkbox: true,
            selectMode: 3,
            select: function (event, data) {
                // A node was activated: display its title:

                GetSelectedTreeNode("readdyTree", "StrAllowedMenus");
                //var node = data.node;
                //if (node.selected) {
                //    var LstSelectedMenu = $("#LstSelectedMenu").val();
                //    LstSelectedMenu += ","+ node.key;
                //    $("#LstSelectedMenu").val(LstSelectedMenu)
                //    alert(LstSelectedMenu);
                //}
                    //console.log(node.key);
                //else
                //    console.log("ko duowc chon");
            },

            
        });

       
    }

);

function GetSelectedTreeNode(treeID,tagInput) {
   // alert(treeID);
    var tree = $("#" + treeID).fancytree("getTree");

    selNodes = tree.getSelectedNodes();
    var lsSelected = "";
    for (i = 0; i < selNodes.length; i++) {
        lsSelected += "," +selNodes[i].key;
    }
    if (lsSelected.length>0)
    lsSelected = lsSelected.substring(1);
    $("#" + tagInput).val(lsSelected);
    


}