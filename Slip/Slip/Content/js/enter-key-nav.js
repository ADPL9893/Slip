/*
 * Enter-to-advance keyboard navigation, shared across every "rd-input" /
 * "chosen" styled form in the app (Lot Entry, Rough Inward, Rough
 * Distribution, Rough Loss, Lot Branch Split, ...). Pressing Enter on a
 * plain field moves focus to the next visible/enabled rd-input field in DOM
 * order; landing on a chosen dropdown opens it automatically - no extra key
 * needed to open it. Once inside an open dropdown, Chosen's own Enter
 * handling picks the highlighted result and closes it (1st Enter) - a 2nd
 * Enter, now that it's closed again, is what actually advances focus
 * onward, so a dropdown always takes one more Enter than a plain field to
 * pass through.
 */
(function ($) {
    "use strict";
    debugger
    function getNavigableFields() {
        // Chosen replaces each native <select> with its own styled
        // .chosen-container and sets the original <select> to display:none -
        // so checking the select's OWN :visible always fails and silently
        // drops every dropdown from this list. Check the wrapping
        // .rd-form-group instead, which stays visible/hidden with its
        // ng-show'd section regardless of what Chosen does to the <select>
        // inside it.
        return $('input.rd-input, select.rd-input')
            .filter(function () {
                return $(this).closest('.rd-form-group').is(':visible');
            })
            .not(':disabled')
            .toArray();
    }

    function openChosenDropdown($select) {
        var $container = $select.next('.chosen-container');
        if (!$container.length || $container.hasClass('chosen-with-drop')) return;
        // This Chosen build binds 'chosen:open' directly on the original
        // <select> specifically so outside code can open its dropdown -
        // more reliable than simulating a mousedown/click on one of its
        // inner elements (which depends on exactly which sub-element the
        // plugin's own handler is bound to).
        $select.trigger('chosen:open');
    }

    function focusNextField(current) {
        var fields = getNavigableFields();
        var idx = fields.indexOf(current);
        if (idx === -1 || idx >= fields.length - 1) return;
        var $next = $(fields[idx + 1]);
        if ($next.is('select')) {
            $next.data('rdJustSelected', false);
            openChosenDropdown($next);
        } else {
            $next.trigger('focus');
            try { $next[0].select(); } catch (e) { /* not a text-selectable input */ }
        }
    }

    $(document).on('keydown', 'input.rd-input', function (e) {
        if (e.which !== 13) return;
        e.preventDefault();
        focusNextField(this);
    });

    $(document).on('keydown', '.chosen-container', function (e) {
        if (e.which !== 13) return;
        var $container = $(this);
        var $select = $container.prev('select');
        var isOpen = $container.hasClass('chosen-with-drop');

        if (isOpen) {
            // 1st Enter while open - let Chosen's own handler (bound
            // directly on the search input, runs before this delegated
            // one) select the highlighted result and close the drop. Just
            // remember a selection happened, so the very next (now-closed)
            // Enter advances instead of re-opening.
            $select.data('rdJustSelected', true);
            return;
        }

        e.preventDefault();
        if ($select.data('rdJustSelected')) {
            $select.data('rdJustSelected', false);
            focusNextField($select[0]);
        } else {
            openChosenDropdown($select);
        }
    });
})(jQuery);
