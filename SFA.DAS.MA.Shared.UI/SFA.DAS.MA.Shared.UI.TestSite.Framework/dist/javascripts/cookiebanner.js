function CookieBanner(module) {
    this.module = module;
    this.settings = {
        seenCookieName: 'DASSeenCookieMessage',
        cookiePolicy: {
            AnalyticsConsent: false,
            MarketingConsent: false
        }
    }
    if (!window.GOVUK.cookie(this.settings.seenCookieName)) {
        this.start()
    }
}

CookieBanner.prototype.start = function () {
    this.module.cookieBanner = this.module.querySelector('.das-cookie-banner')
    this.module.cookieBannerInnerWrap = this.module.querySelector('.das-cookie-banner__wrapper')
    this.module.cookieBannerConfirmationMessage = this.module.querySelector('.das-cookie-banner__confirmation')
    this.setupCookieMessage()
}

CookieBanner.prototype.setupCookieMessage = function () {
    this.module.hideLink = this.module.querySelector('button[data-hide-cookie-banner]')
    this.module.acceptCookiesButton = this.module.querySelector('button[data-accept-cookies]')

    if (this.module.hideLink) {
        this.module.hideLink.addEventListener('click', this.hideCookieBanner.bind(this))
    }

    if (this.module.acceptCookiesButton) {
        this.module.acceptCookiesButton.addEventListener('click', this.acceptAllCookies.bind(this))
    }
    this.showCookieBanner()
}

CookieBanner.prototype.showCookieBanner = function () {
    var cookiePolicy = this.settings.cookiePolicy;
    this.module.cookieBanner.style.display = 'block';
    Object.keys(cookiePolicy).forEach(function (cookieName) {
        window.GOVUK.cookie(cookieName, cookiePolicy[cookieName].toString(), { days: 365 })
    });
}

CookieBanner.prototype.hideCookieBanner = function () {
    this.module.cookieBanner.style.display = 'none';
    window.GOVUK.cookie(this.settings.seenCookieName, true, { days: 365 })
}

CookieBanner.prototype.acceptAllCookies = function () {
    this.module.cookieBannerInnerWrap.style.display = 'none';
    this.module.cookieBannerConfirmationMessage.style.display = 'block';

    window.GOVUK.cookie(this.settings.seenCookieName, true, { days: 365 })

    Object.keys(this.settings.cookiePolicy).forEach(function (cookieName) {
        window.GOVUK.cookie(cookieName, true, { days: 365 })
    });
}

window.GOVUK = window.GOVUK || {}

window.GOVUK.cookie = function (name, value, options) {
    if (typeof value !== 'undefined') {
        if (value === false || value === null) {
            return window.GOVUK.setCookie(name, '', { days: -1 })
        } else {
            // Default expiry date of 30 days
            if (typeof options === 'undefined') {
                options = { days: 30 }
            }
            return window.GOVUK.setCookie(name, value, options)
        }
    } else {
        return window.GOVUK.getCookie(name)
    }
}

window.GOVUK.setCookie = function (name, value, options) {
    if (typeof options === 'undefined') {
        options = {}
    }
    var cookieString = name + '=' + value + '; path=/'

    if (options.days) {
        var date = new Date()
        date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000))
        cookieString = cookieString + '; expires=' + date.toGMTString()
    }
    if (!options.domain) {
        options.domain = window.GOVUK.getDomain()
    }

    if (document.location.protocol === 'https:') {
        cookieString = cookieString + '; Secure'
    }
    document.cookie = cookieString + ';domain=' + options.domain
}

window.GOVUK.getCookie = function (name) {
    var nameEQ = name + '='
    var cookies = document.cookie.split(';')
    for (var i = 0, len = cookies.length; i < len; i++) {
        var cookie = cookies[i]
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1, cookie.length)
        }
        if (cookie.indexOf(nameEQ) === 0) {
            return decodeURIComponent(cookie.substring(nameEQ.length))
        }
    }
    return null
}

window.GOVUK.getDomain = function () {
    return window.location.hostname !== 'localhost'
        ? '.' + window.location.hostname.slice(window.location.hostname.indexOf('.') + 1)
        : window.location.hostname;
}

var $cookieBanner = document.querySelector('[data-module="cookie-banner"]');
if ($cookieBanner != null) {
    new CookieBanner($cookieBanner);
}
