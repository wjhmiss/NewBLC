/*!
 * native-uploadify.js  (v2)
 * Drop-in replacement for jquery.uploadify.js (Flash based, broken in modern browsers).
 * Emulates the uploadify API subset used by this project, so existing page code like
 *   $("#id").uploadify({ uploader: ..., buttonText: ..., onUploadSuccess: ... })
 *   $("#id").uploadify('destroy')
 * keeps working without any page-code change.
 *
 * How it opens the file dialog:
 *   Modern browsers (window.FormData available): a transparent <input type="file">
 *   is overlaid on top of the button, so the user's click lands directly on a real
 *   input and the file dialog opens natively (no programmatic click tricks at all).
 *   A guarded programmatic click is kept as fallback for clicks that miss the input.
 *   Old engines without FormData (IE8/IE9/compat mode): the real input lives inside a
 *   hidden multipart <form> targeting a hidden <iframe>; on file selection the form is
 *   submitted and the iframe response is passed to onUploadSuccess.
 *
 * The file is posted as multipart field "Filedata" (uploadify default), matching the
 * backend action Upload(HttpPostedFileBase Filedata, ...).
 * Rendering reuses uploadify.css classes (.uploadify / .uploadify-button / .uploadify-queue),
 * so the existing stylesheet include must stay in place.
 *
 * Supported options:
 *   uploader, method, formData, fileObjName, multi, auto, buttonText, width, height,
 *   swf (ignored), onFallback (reserved, not required any more), onSelect,
 *   onUploadStart, onUploadSuccess, onUploadError, onUploadComplete
 * Supported methods:
 *   'destroy', 'settings', 'upload'
 * Requires: jQuery 1.7+ (tested with 1.8.2).
 */
(function ($) {
    if (!$) { return; }

    var counter = 0;

    function fileData(f) {
        return { name: f.name, size: f.size, type: f.type };
    }

    function basename(p) {
        return String(p || '').split(/[\\\/]/).pop();
    }

    function buildInstance($el, opts) {
        var o = $.extend({
            uploader: '',
            method: 'post',
            formData: {},
            fileObjName: 'Filedata',
            multi: false,
            auto: true,
            buttonText: 'SELECT FILES',
            width: 120,
            height: 30,
            swf: '',
            onFallback: null,
            onSelect: null,
            onUploadStart: null,
            onUploadSuccess: null,
            onUploadError: null,
            onUploadComplete: null
        }, opts || {});

        // if a previous instance exists on the same element, destroy it first
        var old = $el.data('nativeUploadify');
        if (old) { old.destroy(); }

        var baseId = ($el.attr('id') || ('nuw' + (++counter))) + '-nu';
        var wrapId = baseId + 'wrap';
        var btnId = baseId + 'button';
        var queueId = baseId + 'queue';
        var w = (+o.width || 120);
        var h = (+o.height || 30);

        var $wrap = $('<div id="' + wrapId + '" class="uploadify" style="display:inline-block;width:' + w + 'px;height:' + h + 'px;vertical-align:middle;"></div>');
        var $btn = $('<div id="' + btnId + '" class="uploadify-button" style="position:relative;height:' + h + 'px;line-height:' + h + 'px;cursor:pointer;"><span class="uploadify-button-text" style="font-size:12px;">' + (o.buttonText || '') + '</span></div>');
        var $queue = $('<div id="' + queueId + '" class="uploadify-queue"></div>');

        $wrap.append($btn);
        $el.hide();
        $el.after($wrap);
        $wrap.after($queue);

        function addQueueItem(name) {
            var $item = $('<div class="uploadify-queue-item"></div>').text(name).append('<div class="uploadify-progress"><div class="uploadify-progress-bar" style="width:0;"></div></div>');
            $queue.append($item);
            return $item;
        }

        function finishQueueItem($item, ok) {
            if (!$item || !$item.length) { return; }
            $item.addClass(ok ? 'completed' : 'uploadify-error');
            window.setTimeout(function () { $item.remove(); }, 3000);
        }

        var inst = {
            settings: function (name, value) {
                if (arguments.length === 1 && typeof name === 'object') {
                    $.extend(o, name);
                } else if (arguments.length >= 2) {
                    o[name] = value;
                } else {
                    return o[name];
                }
            },
            upload: function () { /* uploads start automatically on file select */ },
            destroy: null
        };

        if (window.FormData) {
            /* ---------- modern path: transparent input overlay + XHR ---------- */
            var $file = $('<input type="file" id="' + baseId + 'file" name="' + (o.fileObjName || 'Filedata') + '" style="position:absolute;left:0;top:0;width:100%;height:100%;opacity:0;filter:alpha(opacity=0);cursor:pointer;border-width:0;padding:0;margin:0;" />');
            if (o.multi) { $file.attr('multiple', 'multiple'); }
            $btn.append($file);

            function xhrUpload(f) {
                if (!o.uploader) { return; }
                var fd = new FormData();
                fd.append(o.fileObjName, f, f.name);
                if (o.formData) {
                    for (var k in o.formData) {
                        if (o.formData.hasOwnProperty(k)) { fd.append(k, o.formData[k]); }
                    }
                }
                var $item = addQueueItem(f.name);
                var $bar = $item.find('.uploadify-progress-bar');
                var xhr = new XMLHttpRequest();
                if (xhr.upload && xhr.upload.addEventListener) {
                    xhr.upload.addEventListener('progress', function (e) {
                        if (e.lengthComputable) { $bar.css('width', Math.round(e.loaded / e.total * 100) + '%'); }
                    });
                }
                xhr.open((o.method || 'post').toUpperCase(), o.uploader, true);
                xhr.onreadystatechange = function () {
                    if (xhr.readyState !== 4) { return; }
                    var ok = xhr.status >= 200 && xhr.status < 400;
                    finishQueueItem($item, ok);
                    if (ok) {
                        if (typeof o.onUploadSuccess === 'function') {
                            try { o.onUploadSuccess(fileData(f), xhr.responseText, true); } catch (e) { }
                        }
                    } else {
                        if (typeof o.onUploadError === 'function') {
                            try { o.onUploadError(fileData(f), xhr.status, xhr.responseText); } catch (e) { }
                        }
                    }
                    if (typeof o.onUploadComplete === 'function') {
                        try { o.onUploadComplete(fileData(f)); } catch (e) { }
                    }
                };
                if (typeof o.onUploadStart === 'function') {
                    try { o.onUploadStart(fileData(f)); } catch (e) { }
                }
                xhr.send(fd);
            }

            $file.on('change.nuUploadify', function () {
                var files = this.files || [];
                for (var i = 0; i < files.length; i++) {
                    if (typeof o.onSelect === 'function') {
                        try { o.onSelect(fileData(files[i])); } catch (e) { }
                    }
                    if (o.auto) { xhrUpload(files[i]); }
                }
                // allow selecting the same file again
                this.value = '';
            });

            // fallback for clicks that somehow miss the overlaid input
            $btn.on('click.nuUploadify', function (e) {
                if (e.target !== $file[0]) {
                    try { $file[0].click(); } catch (ex) { }
                }
            });

            inst.destroy = function () {
                $file.off('.nuUploadify');
                $btn.off('.nuUploadify');
                $wrap.remove();
                $queue.remove();
                $el.show();
                $el.removeData('nativeUploadify');
            };
        }
        else {
            /* ---------- legacy path: hidden multipart form + hidden iframe (IE8/9/compat) ---------- */
            var frameName = baseId + 'frame';
            var $frame = $('<iframe id="' + frameName + '" name="' + frameName + '" src="about:blank" style="display:none;"></iframe>');
            var $form = $('<form method="post" enctype="multipart/form-data" target="' + frameName + '" style="display:none;"></form>').attr('action', o.uploader);
            var $file = $('<input type="file" name="' + (o.fileObjName || 'Filedata') + '" style="display:none;" />');
            $form.append($file);
            if (o.formData) {
                for (var k2 in o.formData) {
                    if (o.formData.hasOwnProperty(k2)) {
                        $form.append($('<input type="hidden" name="' + k2 + '" />').attr('value', o.formData[k2]));
                    }
                }
            }
            // body-level form/iframe to avoid nested-form markup problems
            $(document.body).append($frame).append($form);

            var busy = false;
            var lastName = '';

            $btn.on('click.nuUploadify', function () {
                try { $file[0].click(); } catch (e) { }
            });

            $file.on('change.nuUploadify', function () {
                if (!o.auto || busy) { return; }
                lastName = basename($file.val());
                if (typeof o.onSelect === 'function') {
                    try { o.onSelect({ name: lastName, size: 0 }); } catch (e) { }
                }
                busy = true;
                var $item = addQueueItem(lastName);
                try {
                    if (typeof o.onUploadStart === 'function') {
                        try { o.onUploadStart({ name: lastName, size: 0 }); } catch (e2) { }
                    }
                    $form[0].submit();
                } catch (e3) {
                    busy = false;
                    finishQueueItem($item, false);
                    if (typeof o.onUploadError === 'function') {
                        try { o.onUploadError({ name: lastName, size: 0 }, 0, String(e3 && e3.message)); } catch (e4) { }
                    }
                    return;
                }
                $frame.off('.nuUploadify').on('load.nuUploadify', function () {
                    if (!busy) { return; }   // ignore the initial about:blank load
                    busy = false;
                    var text = '';
                    try { text = this.contentWindow.document.body.innerHTML; } catch (e5) { }
                    finishQueueItem($item, true);
                    if (typeof o.onUploadSuccess === 'function') {
                        try { o.onUploadSuccess({ name: lastName, size: 0 }, text, true); } catch (e6) { }
                    }
                    if (typeof o.onUploadComplete === 'function') {
                        try { o.onUploadComplete({ name: lastName, size: 0 }); } catch (e7) { }
                    }
                    try { $file.val(''); } catch (e8) { }
                });
            });

            inst.destroy = function () {
                $btn.off('.nuUploadify');
                $file.off('.nuUploadify');
                $frame.off('.nuUploadify');
                $frame.remove();
                $form.remove();
                $wrap.remove();
                $queue.remove();
                $el.show();
                $el.removeData('nativeUploadify');
            };
        }

        $el.data('nativeUploadify', inst);
        return inst;
    }

    $.fn.uploadify = function () {
        var arg0 = arguments[0];
        if (typeof arg0 === 'string') {
            var method = arg0;
            var extra = Array.prototype.slice.call(arguments, 1);
            var ret;
            this.each(function () {
                var inst = $(this).data('nativeUploadify');
                if (!inst) { return; }
                if (method === 'destroy') {
                    inst.destroy();
                } else if (method === 'settings') {
                    ret = inst.settings.apply(inst, extra);
                } else if (method === 'upload') {
                    inst.upload();
                }
            });
            return method === 'destroy' ? this : ret;
        }
        return this.each(function () {
            buildInstance($(this), arg0);
        });
    };
})(window.jQuery);
