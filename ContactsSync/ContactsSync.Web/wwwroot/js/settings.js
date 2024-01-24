(function ($) {

    $(function () {
        $("#ProviderName").change(function () {
            $("#dingtalkForm,#weworkForm").toggleClass("d-none");
        });

        $("form").on("submit", function (event) {
            event.preventDefault();
            if (!$(this).valid()) {
                return;
            }
            let syncPeriod = $("#SyncPeriod").val();
            if (!/^[1-9]\d*$/.test(syncPeriod)) {
                return;
            }
            let data = {
                ProviderName: $("#ProviderName").val(),
                SyncPeriod: $("#SyncPeriod").val()

            };
            let formData = $(this).serializeFormToObject();
            let keys = Object.keys(formData);
            if (keys.length > 0) {

                data.ProviderConfig = formData[keys[0]];
            }
            console.log(data);
            $.ajax({
                url: "/SaveSettings",
                type: "POST",
                contentType: "application/json;charset=UTF-8",
                dataType: "json",
                data: JSON.stringify(data),
                success: function (data) {
                    window.location = "/"

                }
            });
        });

    });

})(jQuery);