function UploadImage() {
    $('.uploadImg').first().click();
}

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            //var b64 = dataUrl.split("base64,")[1];
            $('.dependentImg')
                .attr('src', e.target.result)
                .show();
        };

        reader.readAsDataURL(input.files[0]);
    }
}