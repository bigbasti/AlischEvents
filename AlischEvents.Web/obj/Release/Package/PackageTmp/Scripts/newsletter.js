/// <reference path="jquery-1.5.1-vsdoc.js" />


$('document').ready(function () {

    var sendbutton = $('#sendtestemail');
    var sendemailbutton = $('#sendemail');
    var testdiv = $('#tester');

    sendbutton.click(function () {

        var newsletterid = $('#Id').val();
        var testaddress = $('#testemail').val();

        sendbutton.hide("1000");

        $.ajax({
            type: "GET",
            url: "/Newsletter/SendTestLetter/" + testaddress + "?nid=" + newsletterid,
            success: function (data) {
                if (data == "true") {
                    testdiv.html("<strong>Der Test-newsletter wurde erfolgreich versandt!</strong><p />Bitte prüfen Sie ihen Posteingang.");

                } else {
                    sendbutton.show("1000");
                    sendbutton.text("Senden fehlgeschlagen! Wiederholen?");
                }
            },
            error: function () {
                sendbutton.show("1000");
                sendbutton.text("Senden fehlgeschlagen! Wiederholen?");
            }
        });
        return false;
    });

    sendemailbutton.click(function () {

        var newsletterid = $('#Id').val();
        sendemailbutton.text("Sende, bitte warten...");

        $.ajax({
            type: "GET",
            url: "/Newsletter/SendNewsletter/" + newsletterid,
            success: function (data) {
                if (data == "true") {
                    sendemailbutton.attr("disabled", "disabled");
                    sendemailbutton.text("Der Newsletter wurde versandt! Bitte prüfen Sie ihen Posteingang.");

                } else {
                    sendemailbutton.text("Senden fehlgeschlagen! Wiederholen?");
                }
            },
            error: function () {
                sendemailbutton.text("Senden fehlgeschlagen! Wiederholen?");
            }
        });
        return false;
    });
});

