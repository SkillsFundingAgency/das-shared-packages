
// used by the cookie banner component

(function (root) {
    'use strict'
    window.GOVUK = window.GOVUK || {}

    var DEFAULT_COOKIE_CONSENT = {
        'AnalyticsConsent': 'true',
        'MarketingConsent': 'false'
    }

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

    window.GOVUK.approveAllCookieTypes = function () {
        Object.keys(DEFAULT_COOKIE_CONSENT).forEach(function (cookie) {
            window.GOVUK.setCookie(cookie, DEFAULT_COOKIE_CONSENT[cookie], { days: 365 })
        });
    }

    window.GOVUK.getConsentCookie = function () {
        var consentCookie = window.GOVUK.cookie('cookie_policy')
        var consentCookieObj

        if (consentCookie) {
            try {
                consentCookieObj = JSON.parse(consentCookie)
            } catch (err) {
                return null
            }

            if (typeof consentCookieObj !== 'object' && consentCookieObj !== null) {
                consentCookieObj = JSON.parse(consentCookieObj)
            }
        } else {
            return null
        }

        return consentCookieObj
    }

    window.GOVUK.setConsentCookie = function (options) {
        var cookieConsent = window.GOVUK.getConsentCookie()

        if (!cookieConsent) {
            cookieConsent = JSON.parse(JSON.stringify(DEFAULT_COOKIE_CONSENT))
        }

        for (var cookieType in options) {
            cookieConsent[cookieType] = options[cookieType]

            // Delete cookies of that type if consent being set to false
            if (!options[cookieType]) {
                for (var cookie in COOKIE_CATEGORIES) {
                    if (COOKIE_CATEGORIES[cookie] === cookieType) {
                        window.GOVUK.cookie(cookie, null)

                        if (window.GOVUK.cookie(cookie)) {

                            document.cookie = cookie + '=;expires=' + new Date() + ';domain=.' + window.GOVUK.getDomain() + ';path=/'
                        }
                    }
                }
            }
        }

        window.GOVUK.setCookie('cookie_policy', JSON.stringify(cookieConsent), { days: 365 })
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

        if (document.location.protocol === 'https:') {
            cookieString = cookieString + '; Secure'
        }

        document.cookie = cookieString + ';domain=.' + window.GOVUK.getDomain()
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
            ? window.location.hostname.slice(window.location.hostname.indexOf('.') + 1)
            : window.location.hostname;
    }
}(window))

function CookieSettings($module) {
    this.$module = $module;
    this.DEFAULT_COOKIE_CONSENT = [
        {
            name: 'AnalyticsConsent',
            value: true
        },
        {
            name: 'MarketingConsent',
            value: false
        }
    ]
    this.start()
}

CookieSettings.prototype.start = function () {

    this.$module.submitSettingsForm = this.submitSettingsForm.bind(this)

    document.querySelector('form[data-module=cookie-settings]').addEventListener('submit', this.$module.submitSettingsForm)

    this.setInitialFormValues()
}

CookieSettings.prototype.setInitialFormValues = function () {

    var cookieSettings = this.DEFAULT_COOKIE_CONSENT

    cookieSettings.forEach(function (cookieSetting) {

        var currentConsentCookie = window.GOVUK.cookie(cookieSetting.name)
        var radioButton

        if (currentConsentCookie === 'true' || cookieSetting.value === true) {
            radioButton = document.querySelector('input[name=cookies-' + cookieSetting.name + '][value=on]')
        } else {
            radioButton = document.querySelector('input[name=cookies-' + cookieSetting.name + '][value=off]')
        }

        radioButton.checked = true

    });

}

CookieSettings.prototype.submitSettingsForm = function (event) {

    event.preventDefault()

    var formInputs = event.target.getElementsByTagName("input")

    for (var i = 0; i < formInputs.length; i++) {
        var input = formInputs[i]
        if (input.checked) {
            var name = input.name.replace('cookies-', '')
            var value = input.value === "on" ? true : false
            window.GOVUK.setCookie(name, value, { days: 365 })
        }
    }

    if (!window.GOVUK.cookie("DASSeenCookieMessage")) {
        window.GOVUK.setCookie("DASSeenCookieMessage", true, { days: 365 })
    }

    this.showConfirmationMessage()

    return false
}

CookieSettings.prototype.showConfirmationMessage = function () {
    var confirmationMessage = document.querySelector('div[data-cookie-confirmation]')
    var previousPageLink = document.querySelector('.cookie-settings__prev-page')

    document.body.scrollTop = document.documentElement.scrollTop = 0

    confirmationMessage.style.display = "block"
}

CookieSettings.prototype.getReferrerLink = function () {
    return document.referrer ? new URL(document.referrer).pathname : false
}


function CookieBanner($module) {
    this.$module = $module;
    this.start()
}

CookieBanner.prototype.start = function () {

    this.$module.hideCookieMessage = this.hideCookieMessage.bind(this)
    this.$module.showConfirmationMessage = this.showConfirmationMessage.bind(this)
    this.$module.setDASSeenCookieMessage = this.setDASSeenCookieMessage.bind(this)

    this.$module.cookieBanner = document.querySelector('.das-cookie-banner')
    this.$module.cookieBannerConfirmationMessage = this.$module.querySelector('.das-cookie-banner__confirmation')

    this.setupCookieMessage()
}

CookieBanner.prototype.setupCookieMessage = function () {
    this.$hideLink = this.$module.querySelector('button[data-hide-cookie-banner]')
    if (this.$hideLink) {
        this.$hideLink.addEventListener('click', this.$module.hideCookieMessage)
    }

    this.$acceptCookiesLink = this.$module.querySelector('button[data-accept-cookies]')
    if (this.$acceptCookiesLink) {
        this.$acceptCookiesLink.addEventListener('click', this.$module.setDASSeenCookieMessage)
    }

    if (!window.GOVUK.cookie('DASSeenCookieMessage')) {
        if (window.GOVUK.cookie('DASSeenCookieMessage') === true) {
            window.GOVUK.cookie('DASSeenCookieMessage', false, { days: 365 })
        }
    }
    this.showCookieMessage()
}

CookieBanner.prototype.showCookieMessage = function () {
    if (!this.isInCookiesPage() && !this.isInIframe()) {
        var showCookieBanner = (this.$module && window.GOVUK.cookie('DASSeenCookieMessage') !== 'true')
        if (showCookieBanner) {
            this.$module.style.display = 'block'
        }
    }
}

CookieBanner.prototype.hideCookieMessage = function (event) {
    if (this.$module) {
        this.$module.style.display = 'none'
        window.GOVUK.cookie('DASSeenCookieMessage', true, { days: 365 })
    }
    if (event.target) {
        event.preventDefault()
    }
}

CookieBanner.prototype.setDASSeenCookieMessage = function () {
    window.GOVUK.approveAllCookieTypes()
    this.$module.showConfirmationMessage()
    this.$module.cookieBannerConfirmationMessage.focus()
    window.GOVUK.cookie('DASSeenCookieMessage', true, { days: 365 })
}

CookieBanner.prototype.showConfirmationMessage = function () {
    this.$cookieBannerMainContent = document.querySelector('.das-cookie-banner__wrapper')
    this.$cookieBannerMainContent.style.display = 'none'
    this.$module.cookieBannerConfirmationMessage.style.display = 'block'
}

CookieBanner.prototype.isInCookiesPage = function () {
    return window.location.pathname === '/cookies'
}

CookieBanner.prototype.isInIframe = function () {
    return window.parent && window.location !== window.parent.location
}

var $cookieBanner = document.querySelector('[data-module="cookie-banner"]');
if ($cookieBanner != null) {
    new CookieBanner($cookieBanner);
}

var $cookieSettings = document.querySelector('[data-module="cookie-settings"]');
if ($cookieSettings != null) {
    new CookieSettings($cookieSettings);
}