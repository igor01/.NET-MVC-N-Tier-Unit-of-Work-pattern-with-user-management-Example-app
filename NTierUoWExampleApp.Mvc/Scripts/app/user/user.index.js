userManager = {
    userId: $('#userId').val(),
    aplication_hostname: $('#ApplicationHostname').val(),
    grid_selector: "#user_grid_table",
    grid_pager_selector: "#user_grid_table_pager",
    search_grid_selector: "#user_grid_search",
    search_grid_btn: "#search_grid_btn",
    clear_search_grid_btn: "#clear_search_grid_btn",
    init: function () {
        this.initGrid();
        this.initGridSearch();
    },
    initGridSearch: function () {
        $(userManager.search_grid_selector).keydown(function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                search_jqgrid(userManager.grid_selector, "user_grid_search");
            }
        });

        $(userManager.search_grid_btn).click(function (e) {
            search_jqgrid(userManager.grid_selector, "user_grid_search");
        });

        $(userManager.clear_search_grid_btn).click(function () {
            $(userManager.search_grid_selector).val("");
            search_jqgrid(userManager.grid_selector, "user_grid_search");
        });

    },
    initGrid: function () {
        $(userManager.grid_selector).jqGrid({
            url: userManager.aplication_hostname + "/User/GetUsers",
            mtype: "GET",
            datatype: "json",
            colNames: ['Id', 'Online status', 'UserName', 'First name', 'Last name', 'Phoe number', 'Status', 'Last Login', 'Actions'],
            colModel: [
                    { name: 'Id', index: 'Id', resizable: false, hidden: true, search: false, sortable: false },
                    { name: 'OnlineStatus', index: 'OnlineStatus', resizable: false, formatter: userManager.userOnlineStatusFormater },
                    { name: 'UserName', index: 'UserName', resizable: false },
                    { name: 'FirstName', index: 'FirstName', resizable: false },
                    { name: 'LastName', index: 'LastName', resizable: false },
                    { name: 'PhoneNumber', index: 'PhoneNumber', resizable: false },
                    { name: 'Status', index: 'Status', resizable: false },
                    { name: 'LastLogin', index: 'LastLogin', resizable: false, formatter: "date", formatoptions: { newformat: "d/m/Y H:i" } },
                    { name: 'Actions', index: '', resizable: false, width: "200px", sortable: false, search: false, formatter: userManager.gridActionsFormat }
            ],
            viewrecords: true,
            emptyrecords: "No data in database",
            rowNum: 10,
            rowList: [10, 20, 30],
            pager: userManager.grid_pager_selector,
            altRows: true,
            shrinkToFit: false,
            multiselect: false,
            multiboxonly: true,
            autowidth: true,
            sortname: "FirstName",
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

        var edit = '<a href="' + userManager.aplication_hostname + '/User/Edit?userId=' + rowObject['Id'] + '" class="btn" title="Edit">' +
                            '<i class="fa fa-pencil-square"></i>' +
                       '</a>';

        var reset = '<a onclick="userManager.resetUserPassword(\'' + rowObject['Id'] + ',' + rowObject['UserName'] + '\');" class="btn" title="Reset password">' +
                        '<i class="fa fa-key"></i>' +
                   '</a>';


        var del = '<a onclick="userManager.deletUser(\'' + rowObject['Id'] + ',' + rowObject['UserName'] + '\');" class="btn" title="Delete">' +
                            '<i class="fa fa-trash"></i>' +
                        '</a>';

        var resend = '<a onclick="userManager.resendToken(\'' + rowObject['Id'] + ',' + rowObject['UserName'] + '\');" class="btn" title="Resend account confirmation token">' +
                           '<i class="fa fa-envelope"></i>' +
                       '</a>';

        var history = '<a onclick="userManager.userBrowsingHistory(\'' + rowObject['Id'] + ',' + rowObject['UserName'] + '\');" class="btn" title="View browsing history">' +
                            '<i class="fa fa-history"></i>' +
                        '</a>';

        var endAction = '</div>';


        var actions = startAction + divOpen + edit + reset + del + resend + history + divClose + endAction;
        return actions;

    },
    userOnlineStatusFormater: function (cellvalue, options, rowObject) {
        //show online for current user
        if (rowObject['Id'] == userManager.userId) {
            return '<div class="user-online-circle"></div>';
        }
        else if (rowObject['OnlineStatus'] == 'Online') {
            return '<div class="user-online-circle"></div>';
        }
        else {
            return '<div class="user-offline-circle"></div>';
        }
    },
    deletUser: function (data) {
        var res = data.split(",");
        var userId = res[0];
        var userName = res[1];
        $("#delete_user_name").text(userName);
        $("#delete_user_id").val(userId);
        $('#DeleteUserModal').modal({ backdrop: 'static', keyboard: true }, 'show');
    },
    onDeleteUserSuccess: function (data) {
        $("#DeleteUserModal").modal("hide");
        if (data.success) {
            $(userManager.grid_selector).trigger("reloadGrid", [{ page: 1, current: true }]);
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
    onDeleteUserError: function (data) {
        $("#DeleteUserModal").modal("hide");

        if (data.message != undefined && data.message != "") {

            var options = getDefaultNotificationSettings();
            options.message = data.message;
            options.type = "danger";
            options.duration = 0;

            showNotification(options);
        }
    },
    resetUserPassword: function (data) {
        var res = data.split(",");
        var userId = res[0];
        var userName = res[1];
        $("#reset_password_user_name").text(userName);
        $("#reset_password_user_id").val(userId);
        $('#ResetPasswordModal').modal({ backdrop: 'static', keyboard: true }, 'show');
    },
    onResetUserPasswordSuccess: function (data) {
        $("#ResetPasswordModal").modal("hide");
        if (data.success) {
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
    onResetUserPasswordError: function (data) {
        $("#ResetPasswordModal").modal("hide");

        if (data.message != undefined && data.message != "") {

            var options = getDefaultNotificationSettings();
            options.message = data.message;
            options.type = "danger";
            options.duration = 0;

            showNotification(options);
        }
    },
    resendToken: function (data) {
        var res = data.split(",");
        var userId = res[0];
        var userName = res[1];
        $("#resend_confirmation_email_user_name").text(userName);
        $("#resend_confirmation_email_user_id").val(userId);
        $('#resend_confirmation_email_modal').modal({ backdrop: 'static', keyboard: true }, 'show');
    },
    onResendTokenSuccess: function (data) {
        $("#resend_confirmation_email_modal").modal("hide");
        if (data.success) {
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
    onResendTokenError: function (data) {
        $("#resend_confirmation_email_modal").modal("hide");

        if (data.message != undefined && data.message != "") {
            var options = getDefaultNotificationSettings();
            options.message = data.message;
            options.type = "danger";
            options.duration = 0;

            showNotification(options);
        }
    },
    userBrowsingHistory: function (data) {
        var res = data.split(",");
        var userId = res[0];
        var userName = res[1];

        $.ajax({
            url: '/User/GetUserBrowsingHistory/',
            data: { userId: userId },
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.success) {
                    userManager.showUserBrowsingHistory(data.history);
                }
                else {
                    if (data.message != undefined && data.message != "") {
                        showNotification(data.message, "info", 0);
                    }
                }
            },
            error: function (data) {
                if (data.message != undefined && data.message != "") {
                    showNotification(data.message, "danger", 0);
                }
            }
        });
    },
    showUserBrowsingHistory: function (userHistory) {
        if (userHistory == undefined || userHistory == null) return false;

        //empty container first
        var container = $('#user_web_browsing_table_data_container');
        container.empty();

        for (index in userHistory) {
            var row = '<tr>' +
                            '<td>' + userHistory[index].DateTimeUtc + '</td>' +
                            '<td>' + userHistory[index].PageUrl + '</td>' +
                            '<td>' + userHistory[index].UserAgent + '</td>' +
                      '</tr>';

            $(row).appendTo(container);
        }

        $('#user_browsing_history_modal').modal({ backdrop: 'static', keyboard: true }, 'show');
    }
}