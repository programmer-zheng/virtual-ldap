$(document).ready(function () {

    $("#btnChangePassword").click(function (e) {
        e.preventDefault();
        if ($("#formChangePassword").valid()) {
            var _data = {
                UserId: $("#UserId").val(),
                OldPassword: $("#OldPassword").val(),
                NewPassword: $("#NewPassword").val(),
                ConfirmPassword: $("#ConfirmPassword").val()
            };
            $.ajax({
                type: 'POST',
                url: '/ChangePassword',
                contentType: 'application/json; charset=UTF-8',
                data: JSON.stringify(_data),
                success: function (data) {
                    $("#resultModal .modal-body").html(data.msg);
                    $("#resultModal").modal("show");
                    $("#formChangePassword").get(0).reset();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $("#resultModal .modal-body").html("密码修改失败");
                    $("#resultModal").modal("show");
                }
            });
        }
    });

    $("#btnDeptUsers").on("click", function () {
        var _userid = $("#UserId").val();
        $.get("/ManageUsers", { "userid": _userid }, function (data) {
            $("#tabDeptUsers>tbody").empty().append(data);
        }, "html")
    });

    // 为本人开通VPN账号
    $("#btnEnableVpnAccount").click(function () {
        var _userid = $("#UserId").val();
        $.get("/EnableVpnAccount", { "userid": _userid }, function (data) {
            $("#resultModal .modal-body").html(data.msg);
            $("#resultModal").modal("show");
        });

    });

    // 查看员工账号详情 
    $("body").on("click", ".detail", function () {
        var $this = $(this);
        var _userid = $this.data("userid");
        $.get("/UserDetail", { "userid": _userid }, function (data) {
            $("#lblName").html(data.name)
            $("#lblPosition").html(data.position)
            $("#lblMobile").html(data.mobile)
            $("#lblJobNumber").html(data.jobNumber)
            $("#lblHireDate").html(data.hireDateStr)
            $("#tboDeptUserId").val(data.userid);
            $("#tboUserName").val(data.userName);

            if (data.accountEnabled) {
                $("#tboUserName").attr("readonly", "readonly");
            }

            $(".modalBtn").data("userid", data.userid);
            $("#detailModal").modal("show");
        });

    });

    // 为员工开通域账号
    $("#btnDeptEnableAccount").click(function () {
        var $this = $(this);
        var _username = $("#tboUserName").val();

        if (/^[a-zA-Z]\w+$/ig.test(_username)) {
            $.get($this.data("url"), { "userid": $this.data("userid"), "username": _username }, function (data) {

                $("#detailModal").modal("hide");
                $("#resultModal .modal-body").html(data.msg);
                $("#resultModal").modal("show");
            });
        } else {
            $("#resultModal .modal-body").html("用户名格式不正确，请输入正确的用户名");
            $("#resultModal").modal("show");
        }
    })


    // 为员工启用VPN账号、重置密码
    $("#btnDeptEnableVPNAccount,#btnDeptResetPassword").click(function () {
        var $this = $(this);
        $.get($this.data("url"), { "userid": $this.data("userid") }, function (data) {
            $("#detailModal").modal("hide");

            $("#resultModal .modal-body").html(data.msg);
            $("#resultModal").modal("show");
        });
    });

    $("#btnSaveOutSideUser").click(function (e) {
        saveOutSideUser(e);
        $("#AddOutSideUserModal").modal('close');
    });
    $("#btnSaveOutSideUserContinue").click(function (e) {
        saveOutSideUser(e);
    });
    function saveOutSideUser(e) {
        e.preventDefault();
        if ($("#formAddOutSideUser").valid()) {

            var _data = {
                Name: $("#tboOutSideName").val(),
                UserName: $("#tboOutSideUserName").val(),
                Email: $("#tboOutSideEmail").val(),
                Mobile: $("#tboOutSideMobile").val(),
                Password: $("#tboOutSidePassword").val(),
                AccountStatus: $("#switchAccountStatus").prop("checked"),
                VpnStatus: $("#switchVpnStatus").prop("checked"),
            };
            console.error(_data);
            $.ajax({
                type: 'POST',
                url: '/AddOutSideUser',
                contentType: 'application/json; charset=UTF-8',
                data: JSON.stringify(_data),
                success: function (data) {
                    //$("#resultModal .modal-body").html(data.msg);
                    //$("#resultModal").modal("show");
                    $("#formAddOutSideUser").get(0).reset();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //$("#resultModal .modal-body").html("密码修改失败");
                    //$("#resultModal").modal("show");
                }
            });
        }
    }
});
