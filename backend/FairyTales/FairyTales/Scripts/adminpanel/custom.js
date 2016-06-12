$(function() {
    $("#content")
        .on("froalaEditor.initialized", function() {
            $("#content").parents("form").on("submit", function() {
                return false;
            });
        })
        .froalaEditor({
            language: "ua",
            enter: $.FroalaEditor.ENTER_P,
            placeholderText: "Введіть будь ласка інформацію",
            height: 500,
            toolbarButtons: ['bold', 'italic', 'underline', 'strikeThrough', '|', 'color', 'paragraphStyle', '|', 'paragraphFormat', 'align', 'formatOL', 'formatUL', 'indent', 'outdent', 'quote', '|', 'fullscreen', '-', 'undo', 'redo', 'clearFormatting', 'selectAll', '|', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', '|', 'html']
        });

    $("#article_excerpt")
        .on("froalaEditor.initialized", function () {
            $("#article_excerpt").parents("form").on("submit", function () {
                return false;
            });
        })
        .froalaEditor({
            language: "ua",
            enter: $.FroalaEditor.ENTER_P,
            placeholderText: "Введіть будь ласка короткий опис",
            height: 200,
            toolbarButtons: ['bold', 'italic', 'underline', 'strikeThrough', '|', 'color', 'paragraphStyle', '|', 'fullscreen']
        });
});