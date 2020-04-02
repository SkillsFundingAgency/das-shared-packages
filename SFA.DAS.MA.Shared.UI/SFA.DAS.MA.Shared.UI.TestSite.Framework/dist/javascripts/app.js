var sfa = sfa || {};

sfa.homePage = {
	init: function() {
		this.submitButtons();
		this.toggleRadios();
	},
	submitButtons: function() {
		var that = this;
		$('#submit-what-youll-need-button').on('click touchstart',
			function(e) {
				var isYesClicked = $('#have-everything').prop('checked');
				if (!isYesClicked) {
					e.preventDefault();
					that.showError();
				}
			});

		$('#submit-confirm-who-you-are-button').on('click touchstart',
			function (e) {
				var isClicked = $('#want-to-add-paye-scheme').prop('checked') || $('#do-not-want-to-add-paye-scheme').prop('checked');
				if (!isClicked) {
					e.preventDefault();
					that.showError();
				}
			});
	},
	showError: function() {
		$('.error-message').removeClass("js-hidden hidden").attr("aria-hidden");
		$('#what-you-need-form').addClass("error");
		$('#do-you-want-to-add-paye-scheme-form').addClass("error");
		$('body').data('shownError', true);
	},
	toggleRadios: function() {
		var radios = $('input[type=radio][name=everything-you-need]');
		radios.on('change',
			function() {
				radios.each(function() {
					if ($(this).prop('checked')) {
						var target = $(this).parent().data("target");
						$("#" + target).removeClass("js-hidden hidden").attr("aria-hidden");
					} else {
						var target = $(this).parent().data("target");
						$("#" + target).addClass("js-hidden hidden").attr("aria-hidden", "true");
					}
				});
			});
	}
};

sfa.navigation = {
    elems: {
        userNav: $('nav#user-nav > ul'),
        levyNav: $('ul#global-nav-links')
    },
    init: function () {
        this.setupMenus(this.elems.userNav);
        this.setupEvents(this.elems.userNav);
    },
    setupMenus: function (menu) {
        menu.find('ul').addClass("js-hidden").attr("aria-expanded", "false");
    },
    setupEvents: function (menu) {
        var that = this;
        menu.find('li.has-sub-menu > a').on('click', function (e) {
            var $that = $(this);
            that.toggleMenu($that, $that.next('ul'));
            e.stopPropagation();
            e.preventDefault();
        });
        // Focusout event on the links in the dropdown menu
        menu.find('li.has-sub-menu > ul > li > a').on('focusout', function (e) {
            // If its the last link in the drop down menu, then close
            var $that = $(this);
            if ($(this).parent().is(':last-child')) {
                that.toggleMenu($that, $that.next('ul'));
            }
        });

    },
    toggleMenu: function (link, subMenu) {
        var $li = link.parent();
        if ($li.hasClass("open")) {
            // Close menu
            $li.removeClass("open");
            subMenu.addClass("js-hidden").attr("aria-expanded", "false");
        } else {
            // Open menu
            this.closeAllOpenMenus();
            $li.addClass("open");
            subMenu.removeClass("js-hidden").attr("aria-expanded", "true");
        }
    },
    closeAllOpenMenus: function () {
        this.elems.userNav.find('li.has-sub-menu.open').removeClass('open').find('ul').addClass("js-hidden").attr("aria-expanded", "false");
        this.elems.levyNav.find('li.open').removeClass('open').addClass("js-hidden").attr("aria-expanded", "false");
    },
    linkSettings: function () {
        var $settingsLink = $('a#link-settings'),
            that = this;
        this.toggleUserMenu();
        $settingsLink.attr("aria-expanded", "false");
        $settingsLink.on('click touchstart', function (e) {
            var target = $(this).attr('href');
            $(this).toggleClass('open');
            that.toggleUserMenu();
            e.preventDefault();
        });
    },
    toggleUserMenu: function () {
        var $userNavParent = this.elems.userNav.parent();
        if ($userNavParent.hasClass("close")) {
            //open it
            $userNavParent.removeClass("close").attr("aria-expanded", "true");
        } else {
            // close it 
            $userNavParent.addClass("close").attr("aria-expanded", "false");
        }
    }
}

sfa.tabs = {
    elems: {
        tabs: $('ul.js-tabs li a'),
        panels: $('.js-tab-pane')
    },
    init: function () {
        if (this.elems.tabs) {
            this.setUpEvents(this.elems.tabs);
            this.hidePanels(this.elems.panels);
        }
        this.elems.tabs.eq(0).click();
    },
    hidePanels: function (panels) {
        panels.hide().attr('aria-hidden', 'true');
    },
    showPanel: function (panel) {
        panel.show().attr('aria-hidden', 'false');
    },
    setUpEvents: function (tabs) {
        var that = this;
        tabs.on('click touchstart', function (e) {

            tabs.removeClass('selected');
            $(this).addClass('selected');

            var target = $(this).attr('href');

            that.hidePanels(that.elems.panels);
            that.showPanel($(target));

            e.preventDefault();
        });
    }
};

sfa.forms = {
    init: function () {
        this.preventDoubleSubmit();
    },
    preventDoubleSubmit: function () {
        var forms = $('form').not('.has-client-side-validation');
        forms.on('submit', function (e) {
            var button = $(this).find('.button');
            button.attr('disabled', 'disabled');
            setTimeout(function () {
                button.removeAttr('disabled');
            }, 20000);
        });
    },
    removeDisabledAttr: function () {
        var btns = $('form').not('.has-client-side-validation').find('.button');
        btns.removeAttr('disabled');
    }
};

// Helper for gta events
sfa.tagHelper = {
    dataLayer: {},
    init: function () {
        window.dataLayer = window.dataLayer || [];
    },
    radioButtonClick: function (eventAction, optionName) {
        dataLayer.push({
            'event': 'dataLayerEvent',
            'eventCat': 'Form Option',
            'eventAct': eventAction,
            'eventLab': optionName
        });
    },
    submitRadioForm: function (eventAction) {

        var optionName = $("input[type='radio']:checked").attr('dataOptionName');

        dataLayer.push({
            'event': 'dataLayerEvent',
            'eventCat': 'Form Submit',
            'eventAct': eventAction,
            'eventLab': optionName
        });
    }
};

sfa.backLink = {
    init: function () {
        var backLink = $('<a>')
            .attr({ 'href': '#', 'class': 'link-back' })
            .text('Back')
            .on('click', function (e) { window.history.back(); e.preventDefault(); });
        $('#js-breadcrumbs').html(backLink);
    }
};

if (localStorage.getItem("answers") === null) {
    localStorage.setItem("answers", JSON.stringify([]));
}

sfa.welcomeWizard = {
    settings :  {
        noSteps: $('#welcome').data('total-steps'),
        accountId: $('#welcome').data('account-id')
    },  
    init: function () {
        var that = this;

        var getAnswers = JSON.parse(localStorage.getItem("answers"));
		var indexAnswer = getIndexOf(this.settings.accountId, getAnswers);
		var answers = {};
		answers.accountId = this.settings.accountId;

		if (indexAnswer > -1) {
		    answers = getAnswers[indexAnswer];
		} else {
		    getAnswers.push(answers);
		}

		var radios = $('#welcome input:radio');

		var score = 0;
        radios.filter(':checked').each(function () {
		    score += parseInt($(this).val(), 10);
		});

		$('#confirmation').hide();
		
        radios.on('change', function () {
            answers[this.name] = this.value;
            var listItem = $(this).closest('.todo-list--item');
            that.radioChange($(this).val(), listItem);
            localStorage.setItem("answers", JSON.stringify(getAnswers));
        });

        $.each(answers, function (i, val) {
            if (i !== 'accountId') {
                if (answers[i]) {
                    var radio = $("input[name='" + i + "'][value=" + val + "]")
                    if (radio.prop('checked') && val == 2) {
                        radio.closest('.todo-list--item').addClass('complete');
                    } else {
                        radio.click();
                    }
                    radio.closest('.todo-list--item').removeClass('js-hidden').attr('aria-hidden', false); // Show the entire row if its hidden
                }
            }
        });

        this.editLinks();
    },
    editLinks: function () {
        var h2s = $('#welcome').find('h2');
        var that = this;
        var editLink = $('<a>')
                        .text('(review this step)')
                        .attr('href', '#')
                        .on('click', function (e) {
                            that.toggleStep($(this).closest('.todo-list--item'));
                            e.preventDefault();
                        });

        h2s.append(editLink);
    },
    toggleStep: function (step) {
        if (step.hasClass('complete')) {
            step.removeClass('complete');
        } else {
            step.addClass('complete');
        }
    },
    showStep: function (step) {
        step.removeClass('js-hidden').attr('aria-hidden', false);
    },
    radioChange: function (radioValue, listItem) {
        var that = this;

        if (radioValue == 2) {
            listItem.addClass('complete');

            // If this step is not the last step, then show the next one 
            if (listItem.data('step') < that.settings.noSteps) {
                that.showStep(listItem.next());
            }

        }

        var score = 0;
        $('#welcome input:radio:checked').each(function () {
            score += parseInt($(this).val(), 10);
        });

        if (score == (that.settings.noSteps * 2)) {
            $('#confirmation').show();
        } else {
            $('#confirmation').hide();
        }
    }
};

getIndexOf = function (accountId, items) {
    var i = 0;
    var len = items.length;
    for (i = 0; i < len; i++) {
        if (accountId === items[i].accountId) {
            return i;
        }
    }
    return -1;
};

if ($('#js-breadcrumbs')) {
    sfa.backLink.init();
}

if ($('#welcome')) {
    sfa.welcomeWizard.init();
}

// Removed disabled attribute from buttons when user leaves the page
window.onunload = function () {
    sfa.forms.removeDisabledAttr();
};

sfa.forms.init();
sfa.navigation.init();
sfa.tabs.init();

$('ul#global-nav-links').collapsableNav();

var selectionButtons = new GOVUK.SelectionButtons("label input[type='radio'], label input[type='checkbox'], section input[type='radio']");
var selectionButtonsOrgType = new GOVUK.SelectionButtons("section input[type='radio']", { parentElem: 'section' });

var showHideContent = new GOVUK.ShowHideContent();
showHideContent.init();

// stop apprentice - show/hide date block
$(".js-enabled #stop-effective").hide().attr("aria-hidden", "true");
$(".js-enabled #stop-today").hide().attr("aria-hidden", "true");


$(".js-enabled #WhenToMakeChange-Immediately").on('click touchstart', (function () {
    $("#stop-today").show().attr("aria-hidden", "false");
}));
$(".js-enabled #WhenToMakeChange-SpecificDate").on('click touchstart', (function () {
    $("#stop-today").hide().attr("aria-hidden", "true");
}));

$(".js-enabled #WhenToMakeChange-Immediately").on('click touchstart', (function () {
    $("#stop-effective").hide().attr("aria-hidden", "true");
}));

$(".js-enabled #WhenToMakeChange-SpecificDate").on('click touchstart', (function () {
    $("#stop-effective").show().attr("aria-hidden", "false");
}));


// cohorts bingo balls - clickable block
$(".clickable").on('click touchstart', (function () {
    window.location = $(this).find("a").attr("href");
    return false;
}));

// apprentice filter page :: expand/collapse functionality
$('.container-head').on('click touchstart', (function () {
    $(this).toggleClass('showHide');
    $(this).next().toggleClass("hideOptions");

}));

// Push confirmation messages to the Google dateLayer array
var successMessage = $('.success-summary h1');
if (successMessage.length > 0) {
    var dataLoadedObj = dataLayer[0];
    if (dataLoadedObj.event === 'dataLoaded') {
        dataLoadedObj.success = successMessage.text();
        dataLayer[0] = dataLoadedObj;
    }
}

// Push error messages to the Google dateLayer array
var errorMessage = $('.error-summary');
if (errorMessage.length > 0) {
    var errorContent = errorMessage.find('ul li a').eq(0).text(),
        dataLoadedObj = dataLayer[0];

    if (dataLoadedObj.event === 'dataLoaded') {
        dataLoadedObj.success = errorContent;
        dataLayer[0] = dataLoadedObj;
    }
}