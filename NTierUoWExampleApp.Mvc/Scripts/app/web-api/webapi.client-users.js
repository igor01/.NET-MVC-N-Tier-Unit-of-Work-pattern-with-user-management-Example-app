clientUserManager = {
    aplication_hostname: $('#ApplicationHostname').val(),
    clientId: $('#clientId').val(),
    grid_selector: "#client_users_grid_table",
    grid_pager_selector: "#client_users_grid_table_pager",
    search_grid_selector: "#grid_search",
    search_grid_btn: "#search_grid_btn",
    clear_search_grid_btn: "#clear_search_grid_btn",
    init: function () {
        this.initGrid();
        this.initGridSearch();
    },
    initGridSearch: function () {
        $(clientUserManager.search_grid_selector).keydown(function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                search_jqgrid(clientUserManager.grid_selector, "grid_search");
            }
        });

        $(clientUserManager.search_grid_btn).click(function (e) {
            search_jqgrid(clientUserManager.grid_selector, "grid_search");
        });

        $(clientUserManager.clear_search_grid_btn).click(function () {
            $(clientUserManager.search_grid_selector).val("");
            search_jqgrid(clientUserManager.grid_selector, "grid_search");
        });
    },
    initGrid: function () {
        $(clientUserManager.grid_selector).jqGrid({
            url: clientUserManager.aplication_hostname + "/WebApi/GetClientUsers/" + clientUserManager.clientId,
            mtype: "GET",
            datatype: "json",
            height: 400,
            colNames: ['RefreshTokenId', 'Name', 'Issued Utc', 'Expires Utc', 'Actions'],
            colModel: [
                    { name: 'RefreshTokenId', index: 'RefreshTokenId', resizable: false, hidden: true, search: false, sortable: false, key: true },
                    { name: 'Name', index: 'Name', resizable: false, searchoptions: { searchOperators: true, sopt: ['eq', 'ne', 'bw', 'ew'] } },
                    { name: 'IssuedUtc', index: 'IssuedUtc', resizable: false, formatter: "date", formatoptions: { newformat: "d/m/Y H:i", srcformat: "m/d/Y" } },
                    { name: 'ExpiresUtc', index: 'ExpiresUtc', resizable: false, formatter: "date", formatoptions: { newformat: "d/m/Y H:i", srcformat: "m/d/Y" } },
                    { name: 'Actions', index: '', resizable: false, sortable: false, search: false, formatter: clientUserManager.gridActionsFormat }
            ],
            viewrecords: true,
            emptyrecords: "No data in database",
            rowNum: 10,
            page: 1,
            rowList: [10, 20, 30],
            pager: clientUserManager.grid_pager_selector,
            altRows: true,
            shrinkToFit: true,
            multiselect: false,
            multiboxonly: true,
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
                }, 0);
            },
            autowidth: true,
            ignoreCase: true,
        }).navGrid(this.grid_pager_selector, { search: false, add: false, edit: false, del: false, refresh: true }, {}, {}, {}, {
            multipleSearch: true, closeAfterSearch: true, recreateFilter: true
        });;
    },
    gridActionsFormat: function (cellvalue, options, rowObject) {
        var divOpen = '<div style="float:left;width:100%;">'
        var divClose = '</div>'

        var startAction = '<div id="jqGrid-action-panel">';

        var revoke = '<a onclick="clientUserManager.revokeUser(\'' + rowObject['RefreshTokenId'] + ',' + rowObject['Name'] + '\');" class="ebBtn" title="Revoke user">' +
                                   '<i class="fa fa-trash"></i>' +
                               '</a>';

        var endAction = '</div>';


        var actions = startAction + divOpen + revoke + divClose + endAction;
        return actions;
    },
    revokeUser: function (data) {
        var res = data.split(",");
        var tokenId = res[0];
        var tokenName = res[1];
        $("#revoke_token_name").text(tokenName);
        $("#revoke_token_id").val(tokenId);
        $('#RevokeClientUserModal').modal({ backdrop: 'static', keyboard: true }, 'show');
    },
    onRevokeUserSuccess: function (data) {
        $("#RevokeClientUserModal").modal("hide");
        if (data.success) {
            $(clientUserManager.grid_selector).trigger("reloadGrid", [{ page: 1, current: true }]);
            if (data.message != undefined && data.message != "") {

                var options = getDefaultNotificationSettings();
                options.message = data.message;
                options.type = "success";
                options.duration = 5000;

                showNotification(options);
            }
        }
        else {
            if (data.message != undefined && data.message != "") {

                var options = getDefaultNotificationSettings();
                options.message = data.message;
                options.type = "warning";
                options.duration = 0;

                showNotification(options);
            }
        }
    },
    onRevokeUserError: function (data) {
        $("#RevokeClientUserModal").modal("hide");
        if (data.message != undefined && data.message != "") {

            var options = getDefaultNotificationSettings();
            options.message = data.message;
            options.type = "danger";
            options.duration = 0;

            showNotification(options);
        }
    }
}