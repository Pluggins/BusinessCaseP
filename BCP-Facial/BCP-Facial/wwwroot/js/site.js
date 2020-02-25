function logout() {
    $('#logoutLabel1').html('<i class="fas fa-spinner fa-spin"></i>');
    $('#logoutLabel2').html('<i class="fas fa-spinner fa-spin"></i>');
    $('#logoutButton1').attr('class', 'nav-link disabled');
    $('#logoutButton2').attr('class', 'btn btn-danger disabled');
    $.ajax({
        url: '/Api/User/Logout',
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (responds) {
            if (responds.result == "OK") {
                $('#logoutLabel1').html('Logged Out');
                $('#logoutLabel2').html('Logged Out');
            }

            setTimeout(logoutRedirect, 1000);
        },
        error: function () {
            $('#logoutLabel1').html('Logout');
            $('#logoutLabel2').html('Logout');
            $('#logoutButton1').attr('class', 'nav-link');
            $('#logoutButton2').attr('class', 'btn btn-danger');
        }
    });
}

function logoutRedirect() {
    window.location.replace("/");
}