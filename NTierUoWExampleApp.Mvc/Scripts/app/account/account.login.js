loginManager = {
    init: function () {
        this.registerListeners();
        $('#resetPasswordForm').hide();
    },
    registerListeners: function () {
        //reset password listener
        $('#forgot_password_link').click(function () {
            $('#ResetPasswordModal').modal({ backdrop: 'static', keyboard: true }, 'show');
        });
    },
    onResetUserPasswordSuccess: function (data) {
        $("#ResetPasswordModal").modal("hide");
        $('#reset_email_field').val('');
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
                options.type = "danger";
                options.duration = 0;

                showNotification(options);
            }
        }
    },
    onResetUserPasswordError: function (data) {
        $("#ResetPasswordModal").modal("hide");
        $('#reset_email_field').val('');
        if (data.message != undefined && data.message != "") {

            var options = getDefaultNotificationSettings();
            options.message = data.message;
            options.type = "danger";
            options.duration = 0;

            showNotification(options);
        }
    }
}