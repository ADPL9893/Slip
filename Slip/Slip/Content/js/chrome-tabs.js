/* ==========================================================================
   CHROME-STYLE TAB VIEW MANAGER FOR ADPL ENTERPRISE ERP
   Isolated Iframes, Dynamic Tabs, Context Menus, and Session Sync
   ========================================================================== */

(function (window, document, $) {
    'use strict';

    // ----------------------------------------------------------------------
    // 1. FRAME ENVIRONMENT DETECTION (Child Iframe vs. Master Shell)
    // ----------------------------------------------------------------------
    var isInsideIframe = false;
    try {
        isInsideIframe = (window.self !== window.top);
    } catch (e) {
        isInsideIframe = true;
    }

    // Immediately mark html element if running inside iframe
    if (isInsideIframe) {
        document.documentElement.classList.add('in-tab-iframe');

        // Expose helper API inside child views
        window.openNewTab = function (title, url, icon) {
            try {
                if (window.top && window.top.ChromeTabs && window.top.ChromeTabs.openTab) {
                    window.top.ChromeTabs.openTab({ title: title, url: url, icon: icon });
                    return;
                }
            } catch (ex) { }
            window.open(url, '_blank');
        };

        window.closeCurrentTab = function () {
            try {
                if (window.top && window.top.ChromeTabs && window.top.ChromeTabs.closeTabByWindow) {
                    window.top.ChromeTabs.closeTabByWindow(window);
                    return;
                }
            } catch (ex) { }
            window.close();
        };

        window.reloadCurrentTab = function () {
            window.location.reload();
        };

        // Forward child show_loader / hide_loader calls to parent top shell if possible
        var origShowLoader = window.show_loader;
        window.show_loader = function (message) {
            try {
                if (window.top && window.top !== window && window.top.show_loader) {
                    window.top.show_loader(message);
                    return;
                }
            } catch (ex) { }
            if (typeof origShowLoader === 'function') {
                origShowLoader(message);
            }
        };

        var origHideLoader = window.hide_loader;
        window.hide_loader = function () {
            try {
                if (window.top && window.top !== window && window.top.hide_loader) {
                    window.top.hide_loader();
                    return;
                }
            } catch (ex) { }
            if (typeof origHideLoader === 'function') {
                origHideLoader();
            }
        };

        // Intercept window.open in child frames so internal URLs open in Chrome Tabs
        var origWindowOpen = window.open;
        window.open = function (url, target, features) {
            if (url && typeof url === 'string') {
                // If it's a print window or explicitly sized popup, let native window.open handle it
                var lowerUrl = url.toLowerCase();
                if (lowerUrl.indexOf('print') !== -1 || (features && features.indexOf('width') !== -1)) {
                    return origWindowOpen.apply(this, arguments);
                }

                var isInternal = false;
                try {
                    var u = new URL(url, window.location.origin);
                    isInternal = (u.origin === window.location.origin);
                } catch (e) {
                    isInternal = (url.indexOf('http') !== 0 || url.indexOf(window.location.host) !== -1);
                }

                if (isInternal && window.top && window.top.ChromeTabs && window.top.ChromeTabs.openTab) {
                    var title = 'Tab';
                    if (window.top.ChromeTabs.deriveTitleFromUrl) {
                        title = window.top.ChromeTabs.deriveTitleFromUrl(url);
                    }
                    window.top.ChromeTabs.openTab({ title: title, url: url, forceReload: true });
                    return null;
                }
            }
            return origWindowOpen.apply(this, arguments);
        };

        // Child frame execution ends here
        return;
    }

    // ----------------------------------------------------------------------
    // 2. MASTER CHROME TABS CONTROLLER (Running in outer window)
    // ----------------------------------------------------------------------
    var ChromeTabs = {
        tabs: [],
        activeTabId: null,
        tabCounter: 0,
        options: {
            maxTabs: 25,
            homeUrl: '/Home/Index',
            homeTitle: 'Home'
        },

        // DOM elements cache
        $bar: null,
        $strip: null,
        $track: null,
        $viewport: null,
        $btnScrollLeft: null,
        $btnScrollRight: null,
        $contextMenu: null,
        $allDropdown: null,
        ctxTargetTabId: null,

        // Initialize the Tab Manager
        init: function (opts) {
            var self = this;
            $.extend(this.options, opts || {});

            this.$bar = $('#chromeTabBar');
            this.$strip = $('#chromeTabsStrip');
            this.$track = $('#chromeTabsTrack');
            this.$viewport = $('#chromeTabsViewport');
            this.$btnScrollLeft = $('#tabScrollLeft');
            this.$btnScrollRight = $('#tabScrollRight');
            this.$contextMenu = $('#chromeTabContextMenu');
            this.$allDropdown = $('#chromeTabsDropdown');

            if (!this.$bar.length || !this.$viewport.length) {
                return;
            }

            this.bindEvents();

            // Do NOT maintain or reopen last tab on full page refresh / initial load
            this.tabs = [];
            this.activeTabId = null;
            this.updateEmptyState();
            document.title = 'ADPL';

            // Keep browser address bar on home URL so reloads always remain clean
            if (window.history && window.history.replaceState) {
                try {
                    window.history.replaceState(null, '', this.options.homeUrl || '/Home/Index');
                } catch (e) { }
            }

            // Sync theme to iframes when dark/light toggle is clicked
            $('#tbThemeToggle').on('click', function () {
                setTimeout(function () {
                    self.syncThemeToIframes();
                }, 50);
            });
        },

        // Normalize URL for deduplication
        normalizeUrl: function (url) {
            if (!url) return '';
            try {
                var u = new URL(url, window.location.origin);
                // Lowercase pathname, trim trailing slash
                var path = u.pathname.toLowerCase().replace(/\/+$/, '');
                return path + u.search;
            } catch (e) {
                return url.toLowerCase().replace(/\/+$/, '');
            }
        },

        // Extract pure path without query for fuzzy matching
        getPathOnly: function (url) {
            if (!url) return '';
            try {
                var u = new URL(url, window.location.origin);
                return u.pathname.toLowerCase().replace(/\/+$/, '');
            } catch (e) {
                return url.split('?')[0].toLowerCase().replace(/\/+$/, '');
            }
        },

        // Derive clean title from URL when not provided
        deriveTitleFromUrl: function (url) {
            if (!url) return 'Tab';
            try {
                var u = new URL(url, window.location.origin);
                var parts = u.pathname.split('/').filter(Boolean);
                if (parts.length > 0) {
                    var action = parts[parts.length - 1];
                    if (action.toLowerCase() === 'index' && parts.length > 1) {
                        action = parts[parts.length - 2];
                    }
                    var formatted = action
                        .replace(/([a-z0-9])([A-Z])/g, '$1 $2')
                        .replace(/_+/g, ' ')
                        .trim();
                    if (formatted) return formatted;
                }
            } catch (e) { }
            return 'Tab';
        },

        // Bind UI and navigation event handlers
        bindEvents: function () {
            var self = this;

            // 1. Intercept topbar menu clicks & logo
            $(document).on('click', '.tb-menu a, .tb-menu-link, .main-header a.logo, .tb-user .tb-dropdown a', function (e) {
                var href = $(this).attr('href');
                if (!href || href === '#' || href.indexOf('javascript:') === 0) {
                    if (href === '#') e.preventDefault();
                    return;
                }

                // Let Logout perform normal navigation
                if (href.indexOf('Logout') !== -1 || href.toLowerCase().indexOf('/login/logout') !== -1) {
                    return;
                }

                e.preventDefault();

                // Close menu dropdowns
                if (typeof window.tbCloseAll === 'function') {
                    window.tbCloseAll();
                } else {
                    $('.tb-dropdown').removeClass('open');
                    $('.tb-menu-btn, .tb-user-btn').attr('aria-expanded', 'false');
                }

                var title = $(this).find('.tb-txt').text().trim() || $(this).text().trim() || 'Tab';
                var iconHtml = $(this).find('.tb-ico').html();
                var icon = iconHtml ? ('<span class="tb-ico-svg">' + iconHtml + '</span>') : '<i class="fa fa-diamond"></i>';

                self.openTab({
                    title: title,
                    url: href,
                    icon: icon
                });
            });

            // 2. Click on tab to activate
            this.$track.on('click', '.chrome-tab', function (e) {
                if ($(e.target).closest('.chrome-tab-close').length) return;
                var tabId = $(this).data('tab-id');
                self.activateTab(tabId);
            });

            // 3. Middle click on tab to close
            this.$track.on('mousedown', '.chrome-tab', function (e) {
                if (e.which === 2) { // Middle click
                    e.preventDefault();
                    var tabId = $(this).data('tab-id');
                    self.closeTab(tabId);
                }
            });

            // 4. Click tab close button
            this.$track.on('click', '.chrome-tab-close', function (e) {
                e.stopPropagation();
                var tabId = $(this).closest('.chrome-tab').data('tab-id');
                self.closeTab(tabId);
            });

            // 5. Right-click context menu on tab
            this.$track.on('contextmenu', '.chrome-tab', function (e) {
                e.preventDefault();
                var tabId = $(this).data('tab-id');
                self.showContextMenu(e.pageX, e.pageY, tabId);
            });

            // Close context menu on outside click
            $(document).on('click', function (e) {
                if (!$(e.target).closest('#chromeTabContextMenu').length) {
                    self.$contextMenu.hide();
                }
                if (!$(e.target).closest('#chromeTabsDropdown, #chromeTabAllBtn').length) {
                    self.$allDropdown.hide();
                }
            });

            // 6. Context menu item clicks
            this.$contextMenu.on('click', '.chrome-ctx-item', function (e) {
                e.preventDefault();
                var action = $(this).data('action');
                var tabId = self.ctxTargetTabId;
                self.$contextMenu.hide();

                if (!tabId) return;

                switch (action) {
                    case 'reload':
                        self.reloadTab(tabId);
                        break;
                    case 'duplicate':
                        self.duplicateTab(tabId);
                        break;
                    case 'close':
                        self.closeTab(tabId);
                        break;
                    case 'close-others':
                        self.closeOtherTabs(tabId);
                        break;
                    case 'close-right':
                        self.closeTabsToRight(tabId);
                        break;
                    case 'close-all':
                        self.closeAllTabs();
                        break;
                }
            });

            // 7. Toolbar Buttons (Reload, Close Others, Fullscreen, All Tabs)
            $('#chromeTabReloadCurrent').on('click', function () {
                if (self.activeTabId) {
                    self.reloadTab(self.activeTabId);
                }
            });

            $('#chromeTabCloseOthers').on('click', function () {
                if (self.activeTabId) {
                    self.closeOtherTabs(self.activeTabId);
                }
            });

            $('#chromeTabFullscreen').on('click', function () {
                self.toggleFullscreen();
            });

            $('#chromeTabAllBtn').on('click', function (e) {
                e.stopPropagation();
                self.toggleAllTabsDropdown();
            });

            // 9. All Tabs Dropdown item click
            this.$allDropdown.on('click', '.chrome-tabs-dd-item', function () {
                var tabId = $(this).data('tab-id');
                self.$allDropdown.hide();
                self.activateTab(tabId);
            });

            // 10. Horizontal scroll buttons
            this.$btnScrollLeft.on('click', function () {
                self.scrollTrack(-200);
            });

            this.$btnScrollRight.on('click', function () {
                self.scrollTrack(200);
            });

            // Mouse wheel scroll on tab strip
            this.$strip.on('wheel', function (e) {
                if (e.originalEvent.deltaY !== 0) {
                    e.preventDefault();
                    self.scrollTrack(e.originalEvent.deltaY > 0 ? 120 : -120);
                }
            });

            // 11. Keyboard Shortcuts (Ctrl+W, Ctrl+Shift+Tab, Ctrl+Tab)
            $(document).on('keydown', function (e) {
                // Alt + W or Ctrl + Alt + W to close current tab
                if ((e.ctrlKey || e.altKey) && (e.key === 'w' || e.key === 'W')) {
                    e.preventDefault();
                    if (self.activeTabId) {
                        self.closeTab(self.activeTabId);
                    }
                }
            });

            // 12. Dropdown hover protection over viewport
            $(document).on('click', '.tb-menu-btn, .tb-user-btn', function () {
                var hasOpen = $('.tb-dropdown.open').length > 0;
                self.$viewport.toggleClass('dropdown-active', hasOpen);
            });

            $(document).on('click', function (e) {
                if (!$(e.target).closest('.tb-menu, .tb-user').length) {
                    self.$viewport.removeClass('dropdown-active');
                }
            });

            // Window resize adjustment
            $(window).on('resize', function () {
                self.updateScrollButtons();
            });
        },

        // Open a new tab or activate if already open
        openTab: function (tabData) {
            var self = this;
            if (!tabData || !tabData.url) return;

            var normUrl = this.normalizeUrl(tabData.url);
            var pathOnly = this.getPathOnly(tabData.url);

            // If title is missing or generic 'New Tab' / 'Tab', derive a friendly title
            if (!tabData.title || tabData.title === 'New Tab' || tabData.title === 'Tab') {
                tabData.title = this.deriveTitleFromUrl(tabData.url);
            }

            // Check if tab with this URL or path is already open
            var existingTab = null;
            // 1. Exact match (URL + query)
            for (var i = 0; i < this.tabs.length; i++) {
                var t = this.tabs[i];
                if (tabData.id && t.id === tabData.id) {
                    existingTab = t;
                    break;
                }
                if (this.normalizeUrl(t.url) === normUrl) {
                    existingTab = t;
                    break;
                }
            }

            // 2. Base path match (re-uses existing module tab, e.g. entry form opened with new ID)
            // unless caller explicitly requested duplicate (allowDuplicate: true)
            if (!existingTab && !tabData.allowDuplicate) {
                for (var j = 0; j < this.tabs.length; j++) {
                    var tj = this.tabs[j];
                    if (this.getPathOnly(tj.url) === pathOnly) {
                        existingTab = tj;
                        break;
                    }
                }
            }

            // If already exists, activate it and reload/navigate if URL changed or forceReload set
            if (existingTab) {
                this.activateTab(existingTab.id);

                var urlChanged = (this.normalizeUrl(existingTab.url) !== normUrl);
                if (urlChanged || tabData.forceReload) {
                    existingTab.url = tabData.url;
                    if (tabData.title && tabData.title !== 'Tab' && tabData.title !== 'New Tab') {
                        this.updateTabTitle(existingTab.id, tabData.title);
                    }
                    var $tab = this.$track.find('.chrome-tab[data-tab-id="' + existingTab.id + '"]');
                    $tab.addClass('loading');
                    var $iframe = $('#tab-frame-' + existingTab.id);
                    $iframe.attr('src', tabData.url);
                }
                return;
            }

            // Check max tabs limit
            if (this.tabs.length >= this.options.maxTabs) {
                alert('Maximum tabs limit reached (' + this.options.maxTabs + '). Please close unused tabs.');
                return;
            }

            // Create new tab object
            this.tabCounter++;
            var tabId = tabData.id || ('tab-' + this.tabCounter + '-' + Date.now());
            var tab = {
                id: tabId,
                title: tabData.title || 'Tab',
                url: tabData.url,
                icon: tabData.icon || '<i class="fa fa-diamond"></i>',
                pinned: !!tabData.pinned,
                isLoaded: false
            };

            this.tabs.push(tab);

            // Render Tab Element in Strip
            var $tab = $(
                '<div class="chrome-tab loading" data-tab-id="' + tab.id + '" title="' + this.escapeHtml(tab.title) + '">' +
                    '<span class="chrome-tab-icon">' + tab.icon + '</span>' +
                    '<span class="chrome-tab-spinner"><i class="fa fa-circle-o-notch"></i></span>' +
                    '<span class="chrome-tab-title">' + this.escapeHtml(tab.title) + '</span>' +
                    '<button type="button" class="chrome-tab-close" title="Close tab">&times;</button>' +
                '</div>'
            );
            this.$track.append($tab);

            // Render Iframe in Viewport
            var $frame = $(
                '<iframe class="chrome-tab-frame" id="tab-frame-' + tab.id + '" name="frame-' + tab.id + '" src="' + tab.url + '" frameborder="0"></iframe>'
            );
            this.$viewport.append($frame);

            // Handle Iframe Load Event
            $frame.on('load', function () {
                self.onIframeLoaded(tab.id, this);
            });

            // Activate tab (default: true)
            if (tabData.activate !== false) {
                this.activateTab(tab.id);
            }

            this.updateEmptyState();
            this.updateScrollButtons();
            this.scrollToTab($tab);
        },

        // Activate an existing tab
        activateTab: function (tabId) {
            var tab = this.getTab(tabId);
            if (!tab) return;

            this.activeTabId = tabId;

            // Highlight tab element
            this.$track.find('.chrome-tab').removeClass('active');
            var $tab = this.$track.find('.chrome-tab[data-tab-id="' + tabId + '"]').addClass('active');

            // Show corresponding iframe
            this.$viewport.find('.chrome-tab-frame').removeClass('active');
            var $frame = $('#tab-frame-' + tabId).addClass('active');

            // Trigger window resize inside child frame so DevExtreme grids auto-fit
            try {
                var win = $frame[0].contentWindow;
                if (win && win.dispatchEvent) {
                    win.dispatchEvent(new Event('resize'));
                }
            } catch (ex) { }

            // Update browser document title
            document.title = (tab.title ? (tab.title + ' - ADPL') : 'ADPL');

            this.scrollToTab($tab);
        },

        // Close a tab
        closeTab: function (tabId) {
            var self = this;
            var tabIndex = this.getTabIndex(tabId);
            if (tabIndex === -1) return;

            var tab = this.tabs[tabIndex];

            // Remove tab DOM and iframe
            this.$track.find('.chrome-tab[data-tab-id="' + tabId + '"]').remove();
            $('#tab-frame-' + tabId).remove();

            // Remove from array
            this.tabs.splice(tabIndex, 1);

            // If active tab was closed, switch to adjacent tab
            if (this.activeTabId === tabId) {
                if (this.tabs.length > 0) {
                    var nextIndex = Math.min(tabIndex, this.tabs.length - 1);
                    this.activateTab(this.tabs[nextIndex].id);
                } else {
                    // No tabs open: reset to empty state without creating a default tab
                    this.activeTabId = null;
                    document.title = 'ADPL';
                    try {
                        if (window.history && window.history.replaceState) {
                            window.history.replaceState(null, '', self.options.homeUrl || '/Home/Index');
                        }
                    } catch (ex) { }
                }
            }

            this.updateEmptyState();
            this.updateScrollButtons();
        },

        // Close all tabs except the specified one
        closeOtherTabs: function (keepTabId) {
            var toRemove = [];
            for (var i = 0; i < this.tabs.length; i++) {
                if (this.tabs[i].id !== keepTabId) {
                    toRemove.push(this.tabs[i].id);
                }
            }
            for (var j = 0; j < toRemove.length; j++) {
                this.closeTab(toRemove[j]);
            }
            this.activateTab(keepTabId);
        },

        // Close tabs to the right of the specified one
        closeTabsToRight: function (targetTabId) {
            var targetIndex = this.getTabIndex(targetTabId);
            if (targetIndex === -1) return;

            var toRemove = [];
            for (var i = targetIndex + 1; i < this.tabs.length; i++) {
                toRemove.push(this.tabs[i].id);
            }
            for (var j = 0; j < toRemove.length; j++) {
                this.closeTab(toRemove[j]);
            }
            this.activateTab(targetTabId);
        },

        // Close all tabs
        closeAllTabs: function () {
            var toRemove = [];
            for (var i = 0; i < this.tabs.length; i++) {
                toRemove.push(this.tabs[i].id);
            }
            for (var j = 0; j < toRemove.length; j++) {
                this.closeTab(toRemove[j]);
            }
        },

        // Update empty state visibility when 0 tabs are open
        updateEmptyState: function () {
            var hasTabs = (this.tabs.length > 0);
            $('#chromeTabsEmptyState').toggle(!hasTabs);
        },

        // Reload a tab
        reloadTab: function (tabId) {
            var tab = this.getTab(tabId);
            if (!tab) return;

            var $tab = this.$track.find('.chrome-tab[data-tab-id="' + tabId + '"]');
            $tab.addClass('loading');

            var $frame = $('#tab-frame-' + tabId);
            try {
                $frame[0].contentWindow.location.reload();
            } catch (ex) {
                $frame.attr('src', tab.url);
            }
        },

        // Duplicate a tab
        duplicateTab: function (tabId) {
            var tab = this.getTab(tabId);
            if (!tab) return;

            this.openTab({
                title: tab.title,
                url: tab.url,
                icon: tab.icon,
                allowDuplicate: true
            });
        },

        // Iframe load handler
        onIframeLoaded: function (tabId, iframeEl) {
            var tab = this.getTab(tabId);
            if (!tab) return;

            tab.isLoaded = true;

            var $tab = this.$track.find('.chrome-tab[data-tab-id="' + tabId + '"]');
            $tab.removeClass('loading');

            try {
                var childDoc = iframeEl.contentDocument || iframeEl.contentWindow.document;
                if (childDoc) {
                    // Check if child redirected to login page (session timeout)
                    var childUrl = childDoc.location.href;
                    if (childUrl.indexOf('/Login') !== -1 || childUrl.indexOf('/login') !== -1) {
                        window.location.href = childUrl;
                        return;
                    }

                    // Update internal tab URL if child navigated
                    var childPath = childDoc.location.pathname + childDoc.location.search;
                    if (childPath && childPath !== tab.url) {
                        tab.url = childPath;
                    }

                    // Extract and update title
                    var docTitle = childDoc.title;
                    if (docTitle && docTitle !== 'ADPL' && docTitle !== 'Home') {
                        var cleanTitle = docTitle.split(' - ')[0].trim();
                        if (cleanTitle) {
                            this.updateTabTitle(tabId, cleanTitle);
                        }
                    }

                    // Propagate current theme to iframe document
                    var curTheme = document.documentElement.getAttribute('data-theme');
                    if (curTheme) {
                        childDoc.documentElement.setAttribute('data-theme', curTheme);
                    } else {
                        childDoc.documentElement.removeAttribute('data-theme');
                    }
                }
            } catch (ex) { }
        },

        // Update tab title dynamically
        updateTabTitle: function (tabId, newTitle) {
            var tab = this.getTab(tabId);
            if (!tab) return;

            tab.title = newTitle;
            var $tab = this.$track.find('.chrome-tab[data-tab-id="' + tabId + '"]');
            $tab.attr('title', newTitle);
            $tab.find('.chrome-tab-title').text(newTitle);

            if (this.activeTabId === tabId) {
                document.title = newTitle + ' - ADPL';
            }
        },

        // Helper: get tab by ID
        getTab: function (tabId) {
            for (var i = 0; i < this.tabs.length; i++) {
                if (this.tabs[i].id === tabId) return this.tabs[i];
            }
            return null;
        },

        // Helper: get tab index by ID
        getTabIndex: function (tabId) {
            for (var i = 0; i < this.tabs.length; i++) {
                if (this.tabs[i].id === tabId) return i;
            }
            return -1;
        },

        // Helper: close tab by its contentWindow
        closeTabByWindow: function (win) {
            for (var i = 0; i < this.tabs.length; i++) {
                var $frame = $('#tab-frame-' + this.tabs[i].id);
                if ($frame.length && $frame[0].contentWindow === win) {
                    this.closeTab(this.tabs[i].id);
                    return;
                }
            }
        },

        // Show right-click context menu
        showContextMenu: function (x, y, tabId) {
            this.ctxTargetTabId = tabId;
            var tab = this.getTab(tabId);

            this.$contextMenu.find('[data-action="close"]').show();

            // Position within window bounds
            var winW = $(window).width();
            var menuW = 190;
            if (x + menuW > winW) {
                x = winW - menuW - 10;
            }

            this.$contextMenu.css({
                top: y + 'px',
                left: x + 'px'
            }).fadeIn(100);
        },

        // Toggle All Tabs Dropdown
        toggleAllTabsDropdown: function () {
            if (this.$allDropdown.is(':visible')) {
                this.$allDropdown.hide();
                return;
            }

            var html = '<div class="chrome-tabs-dd-header">Open Tabs (' + this.tabs.length + ')</div>';
            for (var i = 0; i < this.tabs.length; i++) {
                var t = this.tabs[i];
                var isActive = (t.id === this.activeTabId);
                html += '<div class="chrome-tabs-dd-item' + (isActive ? ' active' : '') + '" data-tab-id="' + t.id + '">' +
                            (t.icon || '<i class="fa fa-diamond"></i>') +
                            '<span class="chrome-tabs-dd-title">' + this.escapeHtml(t.title) + '</span>' +
                        '</div>';
            }

            this.$allDropdown.html(html).fadeIn(120);
        },

        // Scroll track horizontally
        scrollTrack: function (amount) {
            var curLeft = this.$strip.scrollLeft();
            this.$strip.animate({ scrollLeft: curLeft + amount }, 150);
        },

        // Scroll so active tab is visible
        scrollToTab: function ($tab) {
            if (!$tab || !$tab.length) return;
            var tabLeft = $tab.position().left;
            var tabWidth = $tab.outerWidth();
            var stripWidth = this.$strip.width();
            var scrollLeft = this.$strip.scrollLeft();

            if (tabLeft < 0) {
                this.$strip.animate({ scrollLeft: scrollLeft + tabLeft - 20 }, 150);
            } else if (tabLeft + tabWidth > stripWidth) {
                this.$strip.animate({ scrollLeft: scrollLeft + (tabLeft + tabWidth - stripWidth) + 30 }, 150);
            }
        },

        // Check if scroll buttons are needed
        updateScrollButtons: function () {
            var trackW = this.$track.outerWidth();
            var stripW = this.$strip.width();
            var isOverflow = (trackW > stripW + 5);

            this.$btnScrollLeft.toggleClass('visible', isOverflow);
            this.$btnScrollRight.toggleClass('visible', isOverflow);
        },

        // Toggle Fullscreen mode
        toggleFullscreen: function () {
            if (!document.fullscreenElement) {
                if (document.documentElement.requestFullscreen) {
                    document.documentElement.requestFullscreen();
                }
            } else {
                if (document.exitFullscreen) {
                    document.exitFullscreen();
                }
            }
        },

        // Synchronize dark/light theme to all open iframes
        syncThemeToIframes: function () {
            var curTheme = document.documentElement.getAttribute('data-theme');
            this.$viewport.find('.chrome-tab-frame').each(function () {
                try {
                    var doc = this.contentDocument || this.contentWindow.document;
                    if (doc && doc.documentElement) {
                        if (curTheme) {
                            doc.documentElement.setAttribute('data-theme', curTheme);
                        } else {
                            doc.documentElement.removeAttribute('data-theme');
                        }
                    }
                } catch (e) { }
            });
        },

        // Escape HTML
        escapeHtml: function (str) {
            if (!str) return '';
            return String(str)
                .replace(/&/g, '&amp;')
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;')
                .replace(/"/g, '&quot;')
                .replace(/'/g, '&#39;');
        }
    };

    // Expose globally
    window.ChromeTabs = ChromeTabs;

})(window, document, jQuery);

