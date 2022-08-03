(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        define([], factory);
    } else if (typeof exports === 'object') {
        module.exports = factory();
    } else {
        root.CY = factory();
    }
}(this, function () {
    var CY = function () { };

    if (jQuery) {
        CY.MVC = false;
        CY.AjaxUrl = "/Cmd.axd";

        CY.AjaxMask = true;
        CY.AjaxMethodName = "action";
        CY.MaskDiv = jQuery("<div id=\"CY_MaskDiv\" style=\"display:none;position: absolute; filter: alpha(opacity=60); background-color: #ccc;z-index: 1002;opacity:0.5; -moz-opacity:0.5;color:black;font-size:18px;padding:15px;font-weight:bolder\">Ajax Response!</div>");

        jQuery(window).ready(function () {
            jQuery("body").append(CY.MaskDiv);
        });

        function MaskShow() {
            CY.MaskDiv.show(300);
            CY.MaskDiv.css("top", (jQuery(window).height() - CY.MaskDiv.height()) / 2);
            CY.MaskDiv.css("left", (jQuery(window).width() - CY.MaskDiv.width()) / 2);
        }

        function MaskHide() {
            CY.MaskDiv.hide(300);
        }


        CY.Ajax = function () {
            var AjaxUrl = CY.AjaxUrl,
                AjaxMethod,
                AjaxHeaders = {},
                AjaxParams = {},
                AjaxCallback,
                Mask = CY.AjaxMask;
            var len = arguments.length;
            if (2 === len) {
                AjaxMethod = arguments[0];
                if (typeof (arguments[1]) === "function") {
                    AjaxCallback = arguments[1];
                } else {
                    AjaxParams = arguments[1];
                }
            } else if (3 === len) {
                AjaxMethod = arguments[0];
                AjaxParams = arguments[1];
                AjaxCallback = arguments[2];
            } else if (4 === len) {
                if (typeof (arguments[1]) === "object" && typeof (arguments[2]) === "object") {
                    AjaxMethod = arguments[0];
                    AjaxHeaders = arguments[1];
                    AjaxParams = arguments[2];
                    AjaxCallback = arguments[3];
                } else {
                    AjaxUrl = arguments[0];
                    AjaxMethod = arguments[1];
                    AjaxParams = arguments[2];
                    AjaxCallback = arguments[3];
                }
            } else if (5 === len) {
                AjaxUrl = arguments[0];
                AjaxMethod = arguments[1];
                AjaxHeaders = arguments[2];
                AjaxParams = arguments[3];
                AjaxCallback = arguments[4];
            } else if (6 === len) {
                AjaxUrl = arguments[0];
                AjaxMethod = arguments[1];
                AjaxHeaders = arguments[2];
                AjaxParams = arguments[3];
                AjaxCallback = arguments[4];
                Mask = arguments[5];
            }
            if (CY.MVC) {
                AjaxUrl = AjaxUrl + "/" + AjaxMethod;
            }
            else {
                AjaxParams[CY.AjaxMethodName] = AjaxMethod;
            }
            jQuery.ajax({
                type: "POST",
                global: false,
                url: AjaxUrl,
                beforeSend: function (XMLHttpRequest) {
                    if (Mask) {
                        MaskShow();
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    if (Mask) {
                        MaskHide();
                    }
                },
                success: function (data,status,xhr) {
                    if (typeof (AjaxCallback) == "function")
                        AjaxCallback(data, xhr);
                },
                crossDomain: (AjaxUrl.toLowerCase().indexOf("//" + location.host.toLowerCase()) == -1),
                xhrFields: {
                    withCredentials: true
                },
                cache: false,
                headers: AjaxHeaders,
                data: AjaxParams,
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=utf-8"
            });
        };

        CY.AjaxJP = function () {
            var AjaxUrl = CY.AjaxUrl,
                AjaxMethod,
                AjaxHeaders = {},
                AjaxParams = {},
                AjaxCallback,
                Mask = CY.AjaxMask;
            var len = arguments.length;
            if (2 === len) {
                AjaxMethod = arguments[0];
                if (typeof (arguments[1]) === "function") {
                    AjaxCallback = arguments[1];
                } else {
                    AjaxParams = arguments[1];
                }
            } else if (3 === len) {
                AjaxMethod = arguments[0];
                AjaxParams = arguments[1];
                AjaxCallback = arguments[2];
            } else if (4 === len) {
                if (typeof (arguments[1]) === "object" && typeof (arguments[2]) === "object") {
                    AjaxMethod = arguments[0];
                    AjaxHeaders = arguments[1];
                    AjaxParams = arguments[2];
                    AjaxCallback = arguments[3];
                } else {
                    AjaxUrl = arguments[0];
                    AjaxMethod = arguments[1];
                    AjaxParams = arguments[2];
                    AjaxCallback = arguments[3];
                }
            } else if (5 === len) {
                AjaxUrl = arguments[0];
                AjaxMethod = arguments[1];
                AjaxHeaders = arguments[2];
                AjaxParams = arguments[3];
                AjaxCallback = arguments[4];
            } else if (6 === len) {
                AjaxUrl = arguments[0];
                AjaxMethod = arguments[1];
                AjaxHeaders = arguments[2];
                AjaxParams = arguments[3];
                AjaxCallback = arguments[4];
                Mask = arguments[5];
            }
            if (CY.MVC) {
                AjaxUrl = AjaxUrl + "/" + AjaxMethod;
            }
            else {
                AjaxParams[CY.AjaxMethodName] = AjaxMethod;
            }
            jQuery.ajax({
                type: "POST",
                global: false,
                async: false,
                url: AjaxUrl,
                beforeSend: function (XMLHttpRequest) {
                    if (Mask) {
                        MaskShow();
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    if (Mask) {
                        MaskHide();
                    }
                },
                success: function (data) {
                    if (typeof (AjaxCallback) === "function")
                        AjaxCallback(data);
                },
                data: AjaxParams,
                crossDomain: true,
                xhrFields: {
                    withCredentials: true
                },
                cache: false,
                dataType: "jsonp",
            });
        };
    }

    CY.EventFix = function (evt) {
        evt = evt || window.event; //标准化事件对象（W3C DOM 和IE DOM ）
        evt.target = evt.target || evt.srcElement; //标准化事件对象属性<引起事件的元素>
        if (evt.target.nodeType == 3) { //nodeType==3代表node.text_node
            evt.target = evt.target.parentNode;
        };//defeat Safari b-u-g
        return evt;
    };

    CY.StopPropagation = function (evt) {
        //evt = evt || window.event;
        if (evt.stopPropagation) { //W3C阻止冒泡方法  
            evt.stopPropagation();
        } else {
            evt.cancelBubble = true; //IE阻止冒泡方法  
        }
        return evt;
    };

    //http://www.jb51.net/article/28737.htm
    CY.IsArray = function (ary) {
        if (ary != this && typeof (ary) != "undefined")
            return Object.prototype.toString.call(ary) == '[object Array]';
        else
            return false;
    };
    CY.IsFunction = function (fun) {
        if (typeof (fun) != "undefined")
            return typeof (fun) == 'function';
        else
            return false;
    };
    CY.Each = function (obj, callback) {
        var length, i = 0;
        if (CY.IsArray(obj)) {
            length = obj.length;
            for (; i < length; i++) {
                if (callback.call(obj[i], i, obj[i]) === false) {
                    break;
                }
            }
        } else {
            for (i in obj) {
                if (callback.call(obj[i], i, obj[i]) === false) {
                    break;
                }
            }
        }
        return obj;
    };

    CY.Clipboard = function (txt) {
        if (txt) {
            if (window.clipboardData) {
                window.clipboardData.clearData();
                //window.clipboardData.setData('text/plain', txt);
                window.clipboardData.setData('text', txt);
                return true;
            } else if (navigator.userAgent.indexOf('Opera') != -1) {
                window.location = txt;
            } else if (window.netscape) {
                try {
                    window.netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
                } catch (e) {
                    return false;
                }
                var clip =
                    Components.classes['@mozilla.org/widget/clipboard;1']
                        .createInstance(Components.interfaces.nsIClipboard);
                if (!clip)
                    return false;
                var trans =
                    Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces
                        .nsITransferable);
                if (!trans)
                    return false;
                trans.addDataFlavor("text/unicode");
                var str = new Object();
                var len = new Object();
                var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces
                    .nsISupportsString);
                var copytext = txt;
                str.data = copytext;
                trans.setTransferData("text/unicode", str, copytext.length * 2);
                var clipid = Components.interfaces.nsIClipboard;
                if (!clip)
                    return false;
                clip.setData(trans, null, clipid.kGlobalClipboard);
                return true;
            }
        } else {
            if (window.clipboardData) {
                return window.clipboardData.getData('text');
            } else if (navigator.userAgent.indexOf('Opera') != -1) {
                return window.location;
            } else if (window.netscape) {
                //try {
                //    window.netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
                //} catch (e) {
                //    return null;
                //}
                //var clip =
                //    Components.classes['@mozilla.org/widget/clipboard;1']
                //        .createInstance(Components.interfaces.nsIClipboard);
                //if (!clip)
                //    return null;
                //var trans =
                //    Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces
                //        .nsITransferable);
                //if (!trans)
                //    return null;
                //trans.addDataFlavor("text/unicode");
                //var str = new Object();
                //var len = new Object();
                //var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces
                //    .nsISupportsString);
                //var copytext = txt;
                //str.data = copytext;
                //trans.setTransferData("text/unicode", str, copytext.length * 2);
                //var clipid = Components.interfaces.nsIClipboard;
                //if (!clip)
                //    return null;
                //return clip.getData(trans, null, clipid.kGlobalClipboard);
            }
        }
    };

    CY.File2Base64 = function (file, callback) {
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function (e) {
            callback(e.target.result);
        }
    };

    (function (CY) {
        var Random = function () { };

        //获取0-maxNum之间的随机数字
        Random.Num = function (maxNum) {
            return Random.NumBetween(0, maxNum);
        };
        //获取min-max之间的随机数字
        Random.NumBetween = function (min, max) {
            max = max + 1;
            return Math.floor(Math.random() * (max - min)) + min;
        };
        //获取长度为length的随机数字
        Random.NumByLength = function (length) {
            var array = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
            var reval = "";
            for (var i = 0; i < length; i++) {
                reval += array[Random.NumBetween(0, array.length - 1)];
            }
            return reval;
        };
        //获取长度为minLength-maxLength之间的随机数
        Random.NumBetweenLength = function (minLength, maxLength) {
            var length = Random.NumBetween(minLength, maxLength);
            return Random.NumByLength(length);
        };
        //获取长度wordLength（数字、字母）组成的字符串
        Random.Word = function (wordLength) {
            var array = [
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
                'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F',
                'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            ];
            var reval = "";
            for (var i = 0; i < wordLength; i++) {
                reval += array[Random.NumBetween(0, array.length - 1)];
            }
            return reval;
        };
        //获取长度为minLength-maxLength之间的随机（数字、字母）组成的字符串
        Random.WordBetweenLength = function (minLength, maxLength) {
            var length = Random.NumBetween(minLength, maxLength);
            return Random.Word(length);
        };
        Random.Guid = function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g,
                function (c) {
                    var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                    return v.toString(16);
                });
        };

        CY.Random = Random;

        return Random;
    })(CY);

    (function (CY) {

        /*浏览获取操作*/
        var Page = function () { };

        Page.Query = function () {
            var s1;
            var q = {}
            var s = document.location.search.substring(1);
            s = s.split("&");
            for (var i = 0, l = s.length; i < l; i++) {
                s1 = s[i].split("=");
                if (s1.length > 1) {
                    var t = s1[1].replace(/\+/g, " ");
                    try {
                        q[s1[0]] = decodeURIComponent(t);
                    } catch (e) {
                        q[s1[0]] = unescape(t);
                    }
                }
            }
            return q;
        };

        Page.Url = function () {
            return window.location.href;
        };

        Page.Domain = function () {
            return window.location.host;
        };

        Page.Back = function () {
            history.go(-1);
        };

        //设备的物理像素分辨率与 CSS 像素分辨率的百分比率
        Page.DevicePixelRatio = function () {
            var ratio = 0;
            var screen = window.screen;
            var ua = navigator.userAgent.toLowerCase();
            if (window.devicePixelRatio !== undefined) {
                ratio = window.devicePixelRatio;
            }
            else if (~ua.indexOf('msie')) {
                if (screen.deviceXDPI && screen.logicalXDPI) {
                    ratio = screen.deviceXDPI / screen.logicalXDPI;
                }
            }
            else if (window.outerWidth !== undefined && window.innerWidth !== undefined) {
                ratio = window.outerWidth / window.innerWidth;
            }

            if (ratio) {
                ratio = Math.round(ratio * 100);
            }
            return ratio;
        };


        //
        Page.AppLaunch = function (schemeurl, appid) {
            //http://www.ampedupdesigns.com/blog/show?bid=67

            function launchAndroidApp(url, packagename) {
                //var androidAppStore = "http://market.android.com/details?id=" + packagename;

                //if (navigator.userAgent.match(/Chrome/)) {
                //    //https://developer.chrome.com/multidevice/android/intents
                //    //https://www.cnblogs.com/alisecurity/p/5417663.html
                //    var intenturl = "intent://{host}{path}/#Intent;scheme={scheme};"
                //    if(packagename){
                //        intenturl += package = " + packagename + ";
                //    }
                //    intenturl += ";end"; 
                //    document.location = intent;
                //}
                //else 
                if (navigator.userAgent.match(/Firefox/)) {
                    launchWebkitApproach(url);
                    setTimeout(function () {
                        launchIframeApproach(url);
                    }, 1500);
                }
                else {
                    launchIframeApproach(url);
                }
            }

            function launchiOSApp(url, appid) {
                var now = new Date().valueOf();
                if (appid) {
                    setTimeout(function () {
                        if (new Date().valueOf() - now > 500) return;
                        var appleAppStoreLink = 'https://itunes.apple.com/cn/app/id' + appid;
                        window.location = appleAppStoreLink;
                    }, 100);
                }
                window.location = url;
            }

            function launchWebkitApproach(url) {
                document.location = url;
            }

            function launchIframeApproach(url) {
                var iframe = document.createElement("iframe");
                iframe.src = url;
                iframe.style.border = "none";
                iframe.style.width = "1px";
                iframe.style.height = "1px";
                document.body.appendChild(iframe);
                window.setTimeout(function () {
                    document.body.removeChild(iframe);
                }, 500);
            }
            var urlreg = /^http(s|):\/\//i;
            if (urlreg.test(schemeurl)) {
                window.location.href = schemeurl;
            } else {
                var iOS = /(iPad|iPhone|iPod)/g.test(navigator.userAgent);
                if (!iOS) {
                    launchAndroidApp(schemeurl, appid);
                }
                else {
                    launchiOSApp(schemeurl, appid);
                }
            }
        };

        //页面是否可见
        Page.IsVisibility = function () {
            // 各种浏览器兼容
            if (typeof (document.hidden) !== "undefined") {
                return !document.hidden;
            } else if (typeof (document.mozHidden) !== "undefined") {
                return !document.mozHidden;
            } else if (typeof (document.msHidden) !== "undefined") {
                return !document.msHidden;
            } else if (typeof (document.webkitHidden) !== "undefined") {
                return !document.webkitHidden;
            }
        };

        //注册可见事件
        Page.VisibilityListener = function (event) {
            // 各种浏览器兼容
            var visibilityChange;
            if (typeof (document.hidden) !== "undefined") {
                visibilityChange = "visibilitychange";
            } else if (typeof (document.mozHidden) !== "undefined") {
                visibilityChange = "mozvisibilitychange";
            } else if (typeof (document.msHidden) !== "undefined") {
                visibilityChange = "msvisibilitychange";
            } else if (typeof (document.webkitHidden) !== "undefined") {
                visibilityChange = "webkitvisibilitychange";
            }
            // 添加监听器
            document.addEventListener(visibilityChange, function () {
                if (CY.IsFunction(event)) {
                    event(Page.IsVisibility());
                }
            }, false);
        };

        if (jQuery) {
            Page.FileSelect = function ($element, callback) {
                if (jQuery("body").find("#CY_Page_FileInput").length == 0) {
                    jQuery("body").append(jQuery("<input id='CY_Page_FileInput' type='file' style='display:none;filter:alpha(opacity=0);opacity:0;width:0;height:0;' />"));
                    jQuery("body").find("#CY_Page_FileInput").on("change",function () {
                        if (this.files.length > 0) {
                            jQuery("[CY_Page_FileSelect]").trigger("fileselected", this.files);
                            $element.removeAttr("CY_Page_FileSelect");
                            //重置value内容,选择相同的文件可以二次运行事件
                            jQuery("body").find("#CY_Page_FileInput").val("");
                        }
                    });
                }
                if ($element) {
                    $element.on("click",function () {
                        $element.one("fileselected", function (e,files) { callback(files); });
                        $element.attr("CY_Page_FileSelect", "");
                        jQuery("body").find("#CY_Page_FileInput").trigger("click");
                    });
                }
            };

            Page.Cookie = function (key, value, options) {
                // Write            
                options = jQuery.extend({ "raw": false, "json": false }, options);
                if (arguments.length > 1 && !CY.IsFunction(value)) {

                    if (typeof options.expires === 'number') {
                        var days = options.expires, t = options.expires = new Date();
                        t.setMilliseconds(t.getMilliseconds() + days * 864e+5);
                    }

                    return (document.cookie = [
                        encode(key), '=', stringifyCookieValue(value),
                        options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
                        options.path ? '; path=' + options.path : '',
                        options.domain ? '; domain=' + options.domain : '',
                        options.secure ? '; secure' : ''
                    ].join(''));
                }
                // Read
                var result = key ? undefined : {},
                    // To prevent the for loop in the first place assign an empty array
                    // in case there are no cookies at all. Also prevents odd result when
                    // calling $.cookie().
                    cookies = document.cookie ? document.cookie.split('; ') : [],
                    i = 0,
                    l = cookies.length;
                for (; i < l; i++) {
                    var parts = cookies[i].split('='),
                        name = decode(parts.shift()),
                        cookie = parts.join('=');
                    if (key === name) {
                        // If second argument (value) is a function it's a converter...
                        result = read(cookie, value);
                        break;
                    }
                    // Prevent storing a cookie that we couldn't decode.
                    if (!key && (cookie = read(cookie)) !== undefined) {
                        result[name] = cookie;
                    }
                }

                return result;

                function encode(s) {
                    //return encodeURIComponent(s);
                    return options.raw ? s : encodeURIComponent(s);
                }

                function decode(s) {
                    //return decodeURIComponent(s);
                    return options.raw ? s : decodeURIComponent(s);
                }

                function stringifyCookieValue(value) {
                    return encode(options.json ? JSON.stringify(value) : String(value));
                }

                function parseCookieValue(s) {
                    if (s.indexOf('"') === 0) {
                        // This is a quoted cookie as according to RFC2068, unescape...
                        s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\');
                    }

                    try {
                        // Replace server-side written pluses with spaces.
                        // If we can't decode the cookie, ignore it, it's unusable.
                        // If we can't parse the cookie, ignore it, it's unusable.
                        s = decodeURIComponent(s.replace(/\+/g, ' '));
                        return options.json ? JSON.parse(s) : s;
                    } catch (e) { }
                }

                function read(s, converter) {
                    var value = options.raw ? s : parseCookieValue(s);
                    return CY.IsFunction(converter) ? converter(value) : value;
                }
            };
        }

        //打印
        Page.Print = function (id /*需要打印的最外层元素ID*/) {
            var el = document.getElementById(id);
            var iframe = document.createElement('IFRAME');
            var doc = null;
            iframe.setAttribute('style', 'position:absolute;width:0px;height:0px;left:-500px;top:-500px;');
            document.body.appendChild(iframe);
            doc = iframe.contentWindow.document;
            doc.write('<div>' + el.innerHTML + '</div>');
            doc.close();
            iframe.contentWindow.focus();
            iframe.contentWindow.print();
            //if (navigator.userAgent.indexOf("MSIE") > 0) {
            if (navigator.userAgent.includes("MSIE")) {
                document.body.removeChild(iframe);
            }
        };

        //加入收藏夹
        Page.AddFavorite = function (surl, stitle) {
            try {
                window.external.addFavorite(surl, stitle);
            } catch (e) {
                try {
                    window.sidebar.addpanel(stitle, surl, "");
                } catch (e) {
                    alert("加入收藏失败,请使用ctrl+d进行添加");
                }
            }
        };

        //设为首页
        Page.SetHome = function (obj, vrl) {
            try {
                obj.style.behavior = 'url(#default#homepage)';
                obj.sethomepage(vrl);
            } catch (e) {
                if (window.netscape) {
                    try {
                        netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
                    } catch (e) {
                        alert("此操作被浏览器拒绝!\n请在浏览器地址栏输入'about:config'并回车\n然后将[signed.applets.codebase_principal_support]的值设置为'true',双击即可。");
                    }
                } else {
                    alert("抱歉，您所使用的浏览器无法完成此操作。\n\n您需要手动设置为首页。");
                }
            }
        };

        Page.NoSelect = function () {
            if (typeof (document.body.onselectstart) != "undefined") {
                // IE下禁止元素被选取        
                document.body.onselectstart = new Function("return false");
            } else {
                // firefox下禁止元素被选取的变通办法        
                document.body.onmousedown = new Function("return false");
                document.body.onmouseup = new Function("return true");
            }
        };

        Page.Browser = {
            versions: function () {
                var u = navigator.userAgent, app = navigator.appVersion;
                return {//移动终端浏览器版本信息  
                    trident: u.indexOf('Trident') > -1, //IE内核  
                    presto: u.indexOf('Presto') > -1, //opera内核  
                    webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核  
                    gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, //火狐内核  
                    mobile: !!u.match(/AppleWebKit.*Mobile.*/) || u.indexOf('Android') > -1 || u.indexOf('iPhone') > -1 || u.indexOf('iPad') > -1, //是否为移动终端  
                    ios: u.indexOf('iPhone') > -1 || u.indexOf('iPad') > -1, //ios终端  
                    android: u.indexOf('Android') > -1, //android终端或者uc浏览器  
                    iPhone: u.indexOf('iPhone') > -1 , //是否为iPhone或者QQHD浏览器  
                    iPad: u.indexOf('iPad') > -1, //是否iPad  
                    // webApp: u.indexOf('Safari') == -1, //是否web应该程序，没有头部与底部
                    // google: u.indexOf('Chrome') > -1,
                    appName: u.indexOf('MicroMessenger') > -1 ? 'WeiXin' : (u.indexOf('AlipayClient') > -1 ? 'Alipay' : (u.indexOf('QQ/') > -1 ? 'QQ' : ''))
                };
            }(),
            language: (navigator.browserLanguage || navigator.language).toLowerCase()
        };


        CY.Page = Page;
    })(CY);

    (function (CY) {
        var Regex = function () { };

        var RegEmail = new RegExp("^[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\\.)+[A-Z]{2,6}$", "i");
        var RegUrl =
            new RegExp(
                "^(http|https|ftp)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&amp;%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.([a-zA-Z]{2,}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&amp;%\$#\=~_\-]+))*$",
                "i");
        var RegIPAddress =
            new RegExp(
                "^(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])$",
                "i");
        var RegDomain = new RegExp("^((?=[a-z0-9-]{1,63}\.)[a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,63}$", "i");
        var RegTel = new RegExp("^[\\d-]+$", "i");

        Regex.IsEmail = function (txt) {
            return txt ? RegEmail.test(txt) : false;
        };

        Regex.IsTel = function (txt) {
            return txt ? RegTel.test(txt) : false;
        };

        Regex.IsUrl = function (txt) {
            return txt ? RegUrl.test(txt) : false;
        };

        Regex.IsDomain = function (txt) {
            return txt ? RegDomain.test(txt) : false;
        };

        Regex.IsIPAddress = function (txt) {
            return txt ? RegIPAddress.test(txt) : false;
        };

        Regex.IsIdCard = function (txt) {
            var city = { 11: "北京", 12: "天津", 13: "河北", 14: "山西", 15: "内蒙古", 21: "辽宁", 22: "吉林", 23: "黑龙江 ", 31: "上海", 32: "江苏", 33: "浙江", 34: "安徽", 35: "福建", 36: "江西", 37: "山东", 41: "河南", 42: "湖北 ", 43: "湖南", 44: "广东", 45: "广西", 46: "海南", 50: "重庆", 51: "四川", 52: "贵州", 53: "云南", 54: "西藏 ", 61: "陕西", 62: "甘肃", 63: "青海", 64: "宁夏", 65: "新疆", 71: "台湾", 81: "香港", 82: "澳门", 91: "国外 " };
            var tip = "";
            var pass = true;

            if (!txt || !/^\d{6}(18|19|20)?\d{2}(0[1-9]|1[012])(0[1-9]|[12]\d|3[01])\d{3}(\d|X)$/i.test(txt)) {
                tip = "身份证号格式错误";
                pass = false;
            }

            else if (!city[txt.substr(0, 2)]) {
                tip = "地址编码错误";
                pass = false;
            }
            else {
                //18位身份证需要验证最后一位校验位
                if (txt.length == 18) {
                    txt = txt.split('');
                    //∑(ai×Wi)(mod 11)
                    //加权因子
                    var factor = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2];
                    //校验位
                    var parity = [1, 0, 'X', 9, 8, 7, 6, 5, 4, 3, 2];
                    var sum = 0;
                    var ai = 0;
                    var wi = 0;
                    for (var i = 0; i < 17; i++) {
                        ai = txt[i];
                        wi = factor[i];
                        sum += ai * wi;
                    }
                    var last = parity[sum % 11];
                    if (parity[sum % 11] != txt[17]) {
                        tip = "校验位错误";
                        pass = false;
                    }
                }
            }
            //if(!pass) alert(tip);
            return pass;
        };

        CY.Regex = Regex;
    })(CY);

    (function (CY) {
        var Arrays = function () { };

        Arrays.Select = function (array, callback) {
            var datas = new Array();
            CY.Each(array, function (i, e) {
                var r = callback(e);
                if (typeof (r) != "undefined") {
                    datas.push(r);
                }
            });
            return datas;
        };

        Arrays.Where = function (array, callback) {
            var datas = new Array();
            CY.Each(array, function (i, e) {
                var r = callback(e);
                if (r) {
                    datas.push(e);
                }
            });
            return datas;
        };

        Arrays.First = function (array, callback) {
            var data;
            CY.Each(array, function (i, e) {
                if (!data) {
                    if (callback) {
                        var r = callback(e);
                        if (r) {
                            data = e;
                        }
                    }
                    else {
                        data = e;
                    }
                }
            });
            return data;
        };

        Arrays.Max = function (array) {
            var data;
            CY.Each(array, function (i, e) {
                var edata = Number(e);
                if (!data) {
                    data = edata;
                }
                else {
                    data = data > edata ? data : edata;
                }
            });
            return data;
        };

        Arrays.Min = function (array, callback) {
            var data;
            CY.Each(array, function (i, e) {
                var edata = Number(e);
                if (!data) {
                    data = edata;
                }
                else {
                    data = data > edata ? edata : data;
                }
            });
            return data;
        };

        Arrays.RemoveAt = function (array, index) {
            array.splice(index, 1);
            return array;
        };

        Arrays.Insert = function (array, index, item) {
            array.splice(index, 0, item);
            return array;
        };

        Arrays.Replace = function (array, index, item) {
            array.splice(index, 1, item);
            return array;
        };

        //是否有重复项
        Arrays.IsRepeat = function (array) {
            var hash = {};
            for (var i in array) {
                if (array.hasOwnProperty(i)) {
                    if (hash[array[i]])
                        return true;
                    hash[array[i]] = true;
                }
            }
            return false;
        };

        //返回去重后的数组
        Arrays.Distinct = function (arr) {
            var res = new Array();
            for (var i = 0; i < arr.length; i++) {
                if (!Arrays.IsRepeat(arr[i])) {
                    res.push(arr[i]);
                }
            }
            return res;
        };

        CY.Arrays = Arrays;
    })(CY);

    (function (CY) {
        var Combinatorics = function () { };
        //返回去重后的数组
        Combinatorics.NoRepeat = function (arr) {
            var res = new Array();
            for (var i = 0; i < arr.length; i++) {
                var list = new Array();
                var isfind = false;
                for (var j = 0; j < arr[i].length; j++) {
                    for (var k = 0; k < list.length; k++) {
                        if (list[k] == arr[i][j]) {
                            isfind = true;
                        }
                    }
                    if (!isfind) {
                        list.push(arr[i][j]);
                    }
                }
                if (!isfind) {
                    res.push(arr[i]);
                }
            }
            return res;
        };


        //组合（不考虑顺序）  组合相当于分组，分成一组就行
        Combinatorics.Combination = function (arr, num) {
            var r = [];
            (function f(t, a, n) {
                if (n == 0) {
                    return r.push(t);
                }
                for (var i = 0, l = a.length; i <= l - n; i++) {
                    f(t.concat(a[i]), a.slice(i + 1), n - 1);
                }
            })([], arr, num);
            return r;
        };

        //排列（考虑顺序）  排列相当于排队，有先后次序
        Combinatorics.Permutation = function (arr, num) {
            var r = [];
            (function f(t, a, n) {
                if (n == 0) {
                    return r.push(t);
                }
                for (var i = 0, l = a.length; i < l; i++) {
                    f(t.concat(a[i]), a.slice(0, i).concat(a.slice(i + 1)), n - 1);
                }
            })([], arr, num);
            return r;
        };

        //数组笛卡尔积  数据项可以重复
        Combinatorics.Combine = function (arr) {
            var len = arr.length;
            // 当数组大于等于2个的时候
            if (len >= 2) {
                // 第一个数组的长度
                var len1 = arr[0].length;
                // 第二个数组的长度
                var len2 = arr[1].length;
                // 2个数组产生的组合数
                var lenBoth = len1 * len2;
                //  申明一个新数组
                var items = new Array(lenBoth);
                // 申明新数组的索引
                var index = 0;
                for (var i = 0; i < len1; i++) {
                    for (var j = 0; j < len2; j++) {
                        if (arr[0][i] instanceof Array) {
                            items[index] = arr[0][i].concat(arr[1][j]);
                        } else {
                            items[index] = [arr[0][i]].concat(arr[1][j]);
                        }
                        index++;
                    }
                }
                var newArr = new Array(len - 1);
                for (var i = 2; i < arr.length; i++) {
                    newArr[i - 1] = arr[i];
                }
                newArr[0] = items;
                return this.Combine(newArr);
            } else {
                return arr[0];
            }
        };

        //排列
        Combinatorics.P = function (m, n) {
            if (m <= 0 || n < 0) {
                return 0;
            }
            if (n == 0) {
                return 1;
            }
            var p = 1;
            while (n--) p *= m--;
            return p;
        };

        //组合
        Combinatorics.C = function (m, n) {
            if (m <= 0 || n < 0) {
                return 0;
            }
            if (n > m) {
                return 0;
            }
            if (n == 0) {
                return 1;
            }
            return this.P(m, n) / this.P(n, n);
        };

        //阶乘   Factorial(5)  5!
        Combinatorics.Factorial = function (n) {
            if (n <= 0) {
                return 0;
            }
            return this.P(n, n);
        };

        //阶乘  
        Combinatorics.Factoradic = function (n, d) {
            var f = 1;
            if (!d) {
                for (d = 1; f < n; f *= ++d);
                if (f > n) f /= d--;
            } else {
                f = this.Factorial(d);
            }
            var result = [0];
            for (; d; f /= d--) {
                result[d] = Math.floor(n / f);
                n %= f;
            }
            return result;
        };

        CY.Combinatorics = Combinatorics;

        return Combinatorics;
    })(CY);

    return CY;
}));

String.prototype.EndsWith = function (str) {
    if (str == null || str == "" || this.length == 0 || str.length > this.length)
        return false;
    if (this.substring(this.length - str.length) == str)
        return true;
    else
        return false;
};

String.prototype.StartsWith = function (str) {
    if (str == null || str == "" || this.length == 0 || str.length > this.length)
        return false;
    if (this.substr(0, str.length) == str)
        return true;
    else
        return false;
};

String.prototype.Contains = function (str) {
    if (str == null || str == "" || this.length == 0 || str.length > this.length)
        return false;
    if (this.indexOf(str) > -1)
        return true;
    else
        return false;
};

String.prototype.Trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
};

String.prototype.TrimEnd = function (str) {
    if (str == null || str == "" || this.length == 0)
        return this;
    if (this.EndsWith(str))
        return this.substr(0, this.length - 1);
    else
        return this;
};

String.prototype.TrimStart = function (str) {
    if (str == null || str == "" || this.length == 0)
        return this;
    if (this.StartsWith(str))
        return this.substr(1, this.length - 1);
    else
        return this;
};

String.prototype.HtmlEncode = function () {
    return this.replace(/&/g, '&amp').replace(/\"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
};

String.prototype.HtmlDecode = function () {
    return this.replace(/&amp;/g, '&').replace(/&quot;/g, '\"').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
};

String.prototype.TextEncode = function () {
    var str = this;
    str = str.replace(/&amp;/gi, '&');
    str = str.replace(/</g, '&lt;');
    str = str.replace(/>/g, '&gt;');
    return str;
};

String.prototype.TextDecode = function () {
    var str = this;
    str = str.replace(/&amp;/gi, '&');
    str = str.replace(/&lt;/gi, '<');
    str = str.replace(/&gt;/gi, '>');
    return str;
};

String.prototype.UrlEncode = function () {
    return escape(this).replace(/\+/g, '%2B').replace(/\"/g, '%22').replace(/\'/g, '%27').replace(/\//g, '%2F');
};

String.prototype.ReplaceAll = function (source, target) {
    var str = this;
    while (str.indexOf(source) >= 0) {
        str = str.replace(source, target);
    }
    return str;
};

String.prototype.ParseInt = function (defaultvalue) {
    if (isNaN(this)) {
        if (defaultvalue) {
            return defaultvalue;
        } else {
            return NaN;
        }
    } else {
        return parseInt(this);
    }
};

String.prototype.ParseFloat = function (precision, defaultvalue) {
    var res = parseFloat(this);
    if (isNaN(this)) {
        if (defaultvalue) {
            res = defaultvalue;
        } else {
            res = NaN;
        }
    }
    if (precision) {
        return res.Format(precision);
    } else {
        return res;
    }
};

String.prototype.ParseDate = function (datestr) {
    var tmp = /\d+(?=\+)/.exec(datestr);
    return new Date(+tmp);
};

Number.prototype.Format = function (precision) {
    var s = this + "";
    if (!precision) precision = 0;
    if (s.indexOf(".") == -1) s += ".";
    s += new Array(precision + 1).join("0");
    if (new RegExp("^(-|\\+)?(\\d+(\\.\\d{0," + (precision + 1) + "})?)\\d*$").test(s)) {
        var s = "0" + RegExp.$2, pm = RegExp.$1, a = RegExp.$3.length, b = true;
        if (a == precision + 2) {
            a = s.match(/\d/g);
            if (parseInt(a[a.length - 1]) > 4) {
                for (var i = a.length - 2; i >= 0; i--) {
                    a[i] = parseInt(a[i]) + 1;
                    if (a[i] == 10) {
                        a[i] = 0;
                        b = i != 1;
                    } else break;
                }
            }
            s = a.join("").replace(new RegExp("(\\d+)(\\d{" + precision + "})\\d$"), "$1.$2");

        }
        if (b) s = s.substr(1);
        return (pm + s).replace(/\.$/, "");
    }
    return this + "";
};

Number.prototype.Format0 = function (precision) {
    var s = this + "";
    if (!precision) precision = 0;
    if (s.indexOf(".") == -1) s += ".";
    s += new Array(precision + 1).join("0");
    if (new RegExp("^(-|\\+)?(\\d+(\\.\\d{0," + (precision + 1) + "})?)\\d*$").test(s)) {
        var s = "0" + RegExp.$2, pm = RegExp.$1, a = RegExp.$3.length, b = true;
        if (a == precision + 2) {
            a = s.match(/\d/g);
            if (parseInt(a[a.length - 1]) > 4) {
                for (var i = a.length - 2; i >= 0; i--) {
                    a[i] = parseInt(a[i]) + 1;
                    if (a[i] == 10) {
                        a[i] = 0;
                        b = i != 1;
                    } else break;
                }
            }
            s = a.join("").replace(new RegExp("(\\d+)(\\d{" + precision + "})\\d$"), "$1.$2");

        }
        if (b) s = s.substr(1);
        return (pm + s).replace(/[0]+$/, "").replace(/\.$/, "");
    }
    return this + "";
};