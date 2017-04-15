//  jqGrid
/* Adjust all jqgrid sizes */
$(window).on('resize', function () {
    window.setTimeout(resize_jqgrids, 0);
});

function resize_jqgrids() {
    if (grid = $('.ui-jqgrid-btable:visible')) {
        grid.each(function (index) {
            gridId = $(this).attr('id');
            gridParentWidth = $('#gbox_' + gridId).parent().width();
            $('#' + gridId).setGridWidth(gridParentWidth);
        });
    }
}

$('.sidebar-toggle').click(function () {
    window.setTimeout(resize_jqgrids, 400);
});

//jqGrid search
function search_jqgrid(grid_selector, searchTextId, optionalReplaceRuleIndexArray) {
    var postData = $(grid_selector).jqGrid("getGridParam", "postData"),
        colModel = $(grid_selector).jqGrid("getGridParam", "colModel"),
        rules = [],
        searchText = $("#" + searchTextId).val(),
        l = colModel.length,
        i,
        cm;
    for (i = 0; i < l; i++) {
        cm = colModel[i];
        //replace col model name if necessary
        if (optionalReplaceRuleIndexArray != undefined && optionalReplaceRuleIndexArray.length > 0) {
            for (var j = 0; j < optionalReplaceRuleIndexArray.length; ++j) {
                if (cm.name == optionalReplaceRuleIndexArray[j].current) {
                    cm.name = optionalReplaceRuleIndexArray[j].replaceWith;
                    break;
                }
            }
        }

        if (cm.search !== false && (cm.stype === undefined || cm.stype === "text")) {
            rules.push({
                field: cm.name,
                op: "cn",
                data: searchText
            });
        }
    }
    postData.filters = JSON.stringify({
        groupOp: "OR",
        rules: rules
    });
    $(grid_selector).jqGrid("setGridParam", { search: true });
    $(grid_selector).trigger("reloadGrid", [{ page: 1, current: true }]);
    return false;
}

//resize jqGrid footer
function SetJqGridFooterHeight() {
    var bdiv = $('.ui-jqgrid-bdiv').css({
        height: "auto"
    });
};

//global notifications

function getDefaultNotificationSettings() {
    var options = {
        message: "",
        type: "",
        duration: 0,
        title: "",
        placement: {
            from: "top",
            align: "center"
        }
    }

    return options;
}

function showNotification(options) {
    //documentation at http://bootstrap-notify.remabledesigns.com/


    $.notify({
        // options
        icon: 'glyphicon glyphicon-warning-sign',
        title: options.title,
        message: options.message,
    }, {
        // settings
        element: 'body',
        position: null,
        type: options.type,
        allow_dismiss: true,
        newest_on_top: false,
        showProgressbar: false,
        placement: {
            from: options.placement.from,
            align: options.placement.align
        },
        offset: 20,
        spacing: 10,
        z_index: 1031,
        delay: options.duration,
        timer: 1000,
        url_target: '_blank',
        mouse_over: null,
        animate: {
            enter: 'animated fadeInDown',
            exit: 'animated fadeOutUp'
        },
        onShow: null,
        onShown: null,
        onClose: null,
        onClosed: null,
        icon_type: 'class',
        template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
            '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
            '<span data-notify="icon"></span> ' +
            '<span data-notify="title">{1}</span> ' +
            '<span data-notify="message">{2}</span>' +
            '<div class="progress" data-notify="progressbar">' +
                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
            '</div>' +
            '<a href="{3}" target="{4}" data-notify="url"></a>' +
        '</div>'
    });
}

$(function () {

    if (globalNotification != undefined) {
        //check for notification

        var options = getDefaultNotificationSettings();


        if (globalNotificationInformationMessage != null && globalNotificationInformationMessage != "") {
            options.message = globalNotificationInformationMessage;
            options.type = "info";
            showNotification(options);
        }

        if (globalNotificationWarningMessage != null && globalNotificationWarningMessage != "") {
            options.message = globalNotificationWarningMessage;
            options.type = "warning";
            showNotification(options);
        }

        if (globalNotificationErrorMessage != null && globalNotificationErrorMessage != "") {
            options.message = globalNotificationErrorMessage;
            options.type = "danger";
            showNotification(options);
        }

        if (globalNotificationSuccessMessage != null && globalNotificationSuccessMessage != "") {
            options.message = globalNotificationSuccessMessage;
            options.type = "success";
            options.duration = 10000;
            showNotification(options);
        }

    }

});