var webapiManager = {
    aplication_hostname: $('#ApplicationHostname').val(),
    grid_selector: "#client_grid_table",
    grid_pager_selector: "#client_grid_table_pager",
    search_grid_selector: "#grid_search",
    search_grid_btn: "#search_grid_btn",
    clear_search_grid_btn: "#clear_search_grid_btn",
    init: function () {
        this.initGrid();
        this.initGridSearch();
    },
    initGridSearch: function () {
        $(webapiManager.search_grid_selector).keydown(function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                search_jqgrid(webapiManager.grid_selector, "grid_search");
            }
        });

        $(webapiManager.search_grid_btn).click(function (e) {
            search_jqgrid(webapiManager.grid_selector, "grid_search");
        });

        $(webapiManager.clear_search_grid_btn).click(function () {
            $(webapiManager.search_grid_selector).val("");
            search_jqgrid(webapiManager.grid_selector, "grid_search");
        });

    },
    initGrid: function () {
        $(webapiManager.grid_selector).jqGrid({
            url: webapiManager.aplication_hostname + "/WebApi/GetClients",
            mtype: "GET",
            datatype: "json",
            colNames: ['ClientId', 'Name', 'Application type', 'Data Access Type', 'Status', 'Refresh token life time', 'Access token life time', 'Allowed origin', 'Actions'],
            colModel: [
                    { name: 'ClientId', index: 'ClientId', resizable: false, hidden: true, search: false, sortable: false, key: true },
                    { name: 'Name', index: 'Name', resizable: false },
                    { name: 'WebApiApplicationTypes', index: 'WebApiApplicationTypes', resizable: false },
                    { name: 'WebApiApplicationDataAccessTypes', index: 'WebApiApplicationDataAccessTypes', resizable: false},
                    {name: 'Status', index: 'Status', resizable: false},
                    { name: 'RefreshTokenLifeTime', index: 'RefreshTokenLifeTime', resizable: false, searchoptions: { searchOperators: true, sopt: ['eq', 'ne'] } },
                    { name: 'AccessTokenLifeTime', index: 'AccessTokenLifeTime', resizable: false, searchoptions: { searchOperators: true, sopt: ['eq', 'ne'] } },
                    { name: 'AllowedOrigin', index: 'AllowedOrigin', resizable: false, searchoptions: { searchOperators: true, sopt: ['eq', 'ne', 'bw', 'ew'] } },
                    { name: 'Actions', index: '', resizable: false, sortable: false, search: false, formatter: webapiManager.gridActionsFormat }                   
            ],
            viewrecords: true,
            emptyrecords: "No data in database",
            rowNum: 10,
            rowList: [10, 20, 30],
            pager: webapiManager.grid_pager_selector,
            altRows: true,
            shrinkToFit: false,
            multiselect: false,
            multiboxonly: true,
            autowidth: true,
            sortname: "Name",
            sortorder: "asc",
            prmNames: {
                search: 'search',
                rows: 'pageSize',
                sort: 'sortColumn',
                order: 'sortOrder'
            },
            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false
            },
            loadComplete: function () {
                setTimeout(function () {
                    SetJqGridFooterHeight();
                }, 0)
            },
            autowidth: true,
            ignoreCase: true,
            gridview: false
        }).navGrid(this.grid_pager_selector, { search: false, add: false, edit: false, del: false, refresh: true }, {}, {}, {}, { multipleSearch: true, closeAfterSearch: true, recreateFilter: true });
    },
    gridActionsFormat: function (cellvalue, options, rowObject) {

        var divOpen = '<div style="float:left;width:100%;">'
        var divClose = '</div>'

        var startAction = '<div id="jqGrid-action-panel">';

        var edit = '<a href="' + webapiManager.aplication_hostname + '/WebApi/EditClient?clientId=' + rowObject['ClientId'] + '" class="btn" title="Edit">' +
                            '<i class="fa fa-pencil-square"></i>' +
                       '</a>';

        var users = '<a href="' + webapiManager.aplication_hostname + '/WebApi/ClientUsers?clientId=' + rowObject['ClientId'] + '" class="btn" title="Edit">' +
                            '<i class="fa fa-users"></i>' +
                       '</a>';


        var del = '<a onclick="webapiManager.deleteClient(\'' + rowObject['ClientId'] + ',' + rowObject['Name'] + '\');" class="btn" title="Delete">' +
                            '<i class="fa fa-trash"></i>' +
                        '</a>';

        

        var endAction = '</div>';


        var actions = startAction + divOpen + edit + users + del + divClose + endAction;
        return actions;

    },
    deleteClient: function (data) {
        var res = data.split(",");
        var clientId = res[0];
        var clientName = res[1];
        $("#delete_client_name").text(clientName);
        $("#delete_client_id").val(clientId);
        $('#DeleteWebApiClientModal').modal({ backdrop: 'static', keyboard: true }, 'show');
    },
    onDeleteClientSuccess: function (data) {
        $("#DeleteWebApiClientModal").modal("hide");
        if (data.success) {
            $(webapiManager.grid_selector).trigger("reloadGrid", [{ page: 1, current: true }]);
            if (data.message != undefined && data.message != "") {
                showNotification(data.message, "info", 5000);
            }
        }
        else {
            if (data.message != undefined && data.message != "") {
                showNotification(data.message, "danger", 0);
            }
        }
    },
    onDeleteClientError: function (data) {
        $("#DeleteWebApiClientModal").modal("hide");
        if (data.message != undefined && data.message != "") {
            showNotification(data.message, "danger", 0);
        }
    },
}