//http://plnkr.co/edit/LcGe64WkNOAJZfqc7Ics?p=preview
var appModule;
try {
    appModule = angular.module('app');
} catch (e) {
    appModule = angular.module('app', []);
}
var CallBack = 0;
var CallBackParam = 0;
var CallonRowSelect = 0;
var UpdateCellChangeUse = 0;

var sSearch = "";
/* Start stable version */
function GriedFill($scope, $http, $q, $interval, uiGridConstants, URL, header, SavePostDataURL, GridOptionName) {
    var paginationOptions = {
        pageNumber: 1,
        iDisplayStart: 0,
        iDisplayLength: 25,
        sSearch: sSearch,
        iSortCols: null
    };
    $scope[GridOptionName] = {
        paginationPageSizes: [5, 25, 50, 75],
        paginationPageSize: paginationOptions.iDisplayLength,
        useExternalPagination: true,
        useExternalSorting: true,
        //  enableFiltering: true,
        useExternalFiltering: true,
        enableColumnResizing: true,
        //  showColumnFooter: true,
        columnDefs:
           header
        ,
        /* START Grid Option  */
        enableGridMenu: true,
        // enableSelectAll: true,
        // exporterMenuPdf: false, // ADD THIS
        exporterCsvFilename: 'myFile.csv',
        exporterPdfDefaultStyle: { fontSize: 9 },
        exporterPdfTableStyle: { margin: [30, 30, 30, 30] },
        exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'red' },
        exporterPdfHeader: { text: "My Header", style: 'headerStyle' },

        exporterPdfFooter: function (currentPage, pageCount) {
            return { text: currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
        },
        exporterPdfCustomFormatter: function (docDefinition) {
            docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
            docDefinition.styles.footerStyle = { fontSize: 10, bold: true };
            return docDefinition;
        },
        exporterPdfOrientation: 'portrait',
        exporterPdfPageSize: 'A4',
        exporterPdfMaxGridWidth: 500,
        exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
        /* END Grid Option  */
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            //$interval(function () {
            //    $scope.gridApi.core.handleWindowResize();
            //}, 500, 10);

            gridApi.cellNav.on.navigate($scope, function (newRowcol, oldRowCol) {
                $scope.rowCol = newRowcol;
            })
            gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                $scope.$apply();
                $scope.UpdateChangeRowInfo(rowEntity);

            });
            $scope.gridApi.core.on.filterChanged($scope, function () {
                var grid = this.grid;
                paginationOptions.sSearch = "";
                for (var i = 0; i < grid.columns.length; i++) {
                    if (grid.columns[i].cellClass == "InQueryInt") {
                        var searchTmp = grid.columns[i].filters[0].term.trim().replace(/,\s*$/, "");
                        if (searchTmp != "") {
                            paginationOptions.sSearch += " AND " + grid.columns[i].colDef["field"] + " in (" + searchTmp + ")";
                        }
                        else {
                            paginationOptions.sSearch = "";
                        }
                    }
                    else {
                        paginationOptions.sSearch += " AND " + grid.columns[i].colDef["field"] + " like '%" + grid.columns[i].filters[0].term + "%'";
                    }
                }
                getPage()
            });
            $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                if (sortColumns.length == 0) {
                    paginationOptions.iSortCols = null;
                } else {
                    paginationOptions.iSortCols = " Order By " + sortColumns[0].field + " " + sortColumns[0].sort.direction;
                }
                getPage();
            });
            gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                paginationOptions.pageNumber = newPage;
                paginationOptions.iDisplayLength = pageSize;
                getPage();
            });
            //gridApi.core.on.renderingComplete($scope, function () {
            //    if (CallBack != 0) {
            //        $scope.CallBack();
            //    }
            //});
        }
    };
    var saveRow = function (rowEntity) {
        var promise = $q.defer();
        $scope.gridApi.rowEdit.setSavePromise(rowEntity, promise.promise);
        $interval(function () {
            var response = $http({
                method: "post",
                async: true,
                url: SavePostDataURL,
                data: rowEntity,
                dataType: "json"
            });
        }, 3000, 1);
    };
    var getPage = function () {
        var url = URL;
        var data = {
            pageNumber: paginationOptions.pageNumber,
            iDisplayLength: paginationOptions.iDisplayLength,
            iDisplayStart: (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength,
            sSearch: paginationOptions.sSearch,
            iSortCols: paginationOptions.iSortCols
        };

        var config = {
            params: data,
            headers: { 'Accept': 'application/json' }
        };
        $http.get(url, config).success(function (data) {
            $scope[GridOptionName].totalItems = data.iTotalDisplayRecords;
            paginationOptions.iDisplayStart = (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength;
            $scope[GridOptionName].data = data.aaData;
        });
    };
    getPage();
}

function DisplaySelectionGried($scope, $http, $q, $interval, uiGridConstants, URL, header, GridOptionName) {
    var paginationOptions = {
        pageNumber: 1,
        iDisplayStart: 0,
        iDisplayLength: 25,
        sSearch: "",
        iSortCols: null
    };
    $scope.RunTimeChecked = false;

    $scope[GridOptionName] = {
        paginationPageSizes: [5, 25, 50, 75],
        paginationPageSize: paginationOptions.iDisplayLength,
        useExternalPagination: true,
        useExternalSorting: true,
        enableFiltering: true,
        useExternalFiltering: true,
        enableColumnResizing: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableGridMenu: true,
        //rowTemplate: rowTemplate(),
        columnDefs:
           header
        ,
        exporterCsvFilename: 'myFile.csv',
        exporterPdfDefaultStyle: { fontSize: 9 },
        exporterPdfTableStyle: { margin: [30, 30, 30, 30] },
        exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'red' },
        exporterPdfHeader: { text: "My Header", style: 'headerStyle' },

        exporterPdfFooter: function (currentPage, pageCount) {
            return { text: currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
        },
        exporterPdfCustomFormatter: function (docDefinition) {
            docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
            docDefinition.styles.footerStyle = { fontSize: 10, bold: true };
            return docDefinition;
        },
        exporterPdfOrientation: 'portrait',
        exporterPdfPageSize: 'A4',
        exporterPdfMaxGridWidth: 500,
        exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),

        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            //$interval(function () {
            //    $scope.gridApi.core.handleWindowResize();
            //}, 500, 10);
            $scope.gridApi.core.on.filterChanged($scope, function () {
                var grid = this.grid;
                paginationOptions.sSearch = "";
                for (var i = 0; i < grid.columns.length; i++) {
                    if (grid.columns[i].filters[0].term != undefined) {

                        if (grid.columns[i].cellClass == "InQueryInt") {
                            var searchTmp = grid.columns[i].filters[0].term.trim().replace(/,\s*$/, "");
                            if (searchTmp != "") {
                                paginationOptions.sSearch += " AND " + grid.columns[i].colDef["field"] + " in (" + searchTmp + ")";
                            }
                            else {
                                paginationOptions.sSearch = "";
                            }
                        }
                        else {
                            paginationOptions.sSearch += " AND " + grid.columns[i].colDef["field"] + " like '%" + grid.columns[i].filters[0].term + "%'";
                        }
                    }
                }
                getDisplayRecords()
            });
            $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                if (sortColumns.length == 0) {
                    paginationOptions.iSortCols = null;
                } else {
                    paginationOptions.iSortCols = " Order By " + sortColumns[0].field + " " + sortColumns[0].sort.direction;
                }
                getDisplayRecords();
            });
            gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                paginationOptions.pageNumber = newPage;
                paginationOptions.iDisplayLength = pageSize;
                getDisplayRecords();
            });
            //gridApi.core.on.renderingComplete($scope, function () {
            //    $interval(function () {
            //        $scope.gridApi.core.handleWindowResize();
            //    }, 500, 10);
            //});

        }
    };
    var getDisplayRecords = function () {
        var url = URL;//'@Url.Action("GetAllData", "Utility")';
        var data = {
            pageNumber: paginationOptions.pageNumber,
            iDisplayLength: paginationOptions.iDisplayLength,
            iDisplayStart: (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength,
            sSearch: paginationOptions.sSearch,
            iSortCols: paginationOptions.iSortCols
        };
        var config = {
            params: data,
            headers: { 'Accept': 'application/json' }
        };
        $http.get(url, config).success(function (data) {
            $scope[GridOptionName].totalItems = data.iTotalDisplayRecords;
            paginationOptions.iDisplayStart = (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength;
            $scope[GridOptionName].data = data.aaData;

            $scope.heightPopup = (($scope[GridOptionName].data.length * 30) + 30);
            $scope.heightPopup += 100;

        });
    };

    getDisplayRecords();
}

function GriedParam($scope, $http, $q, $interval, uiGridConstants, URL, header, GridOptionName) {
    loaderShow();
    var paginationOptions = {
        pageNumber: 1,
        iDisplayStart: 0,
        iDisplayLength: 25,
        sSearch: sSearch,
        iSortCols: null
    };

    $scope[GridOptionName] = {
        paginationPageSizes: [5, 25, 50, 75, 100, 200, 500, 700],
        paginationPageSize: paginationOptions.iDisplayLength,
        useExternalPagination: true,
        useExternalSorting: true,
        //  enableFiltering: true,
        useExternalFiltering: true,
        enableColumnResizing: true,
        enableRowSelection: true,
        rowHeight: 25,
        //  showColumnFooter: true,
        columnDefs:
           header
        ,
        /* START Grid Option  */
        enableGridMenu: true,
        // enableSelectAll: true,
        // exporterMenuPdf: false, // ADD THIS
        exporterCsvFilename: 'myFile.csv',
        exporterPdfDefaultStyle: { fontSize: 9 },
        exporterPdfTableStyle: { margin: [30, 30, 30, 30] },
        exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'red' },
        exporterPdfHeader: { text: "My Header", style: 'headerStyle' },

        exporterPdfFooter: function (currentPage, pageCount) {
            return { text: currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
        },
        exporterPdfCustomFormatter: function (docDefinition) {
            docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
            docDefinition.styles.footerStyle = { fontSize: 10, bold: true };
            return docDefinition;
        },
        exporterPdfOrientation: 'portrait',
        exporterPdfPageSize: 'A4',
        exporterPdfMaxGridWidth: 500,
        exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
        /* END Grid Option  */
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.gridApiParam = gridApi;

            //gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
            //    $scope.$apply();
            //    if (UpdateCellChangeUse != 0) {
            //        $scope.UpdateChangeRowInfoParam(rowEntity, colDef, newValue, oldValue);
            //    }

            //});

            $scope.gridApi.core.on.filterChanged($scope, function () {
                var grid = this.grid;
                paginationOptions.sSearch = "";
                for (var i = 0; i < grid.columns.length; i++) {
                    if (grid.columns[i].cellClass == "InQueryInt") {
                        var searchTmp = grid.columns[i].filters[0].term.trim().replace(/,\s*$/, "");
                        if (searchTmp != "") {
                            paginationOptions.sSearch += " AND " + grid.columns[i].colDef["field"] + " in (" + searchTmp + ")";
                        }
                        else {
                            paginationOptions.sSearch = "";
                        }
                    }
                    else {
                        paginationOptions.sSearch += " AND " + grid.columns[i].colDef["field"] + " like '%" + grid.columns[i].filters[0].term + "%'";
                    }
                }
                getDisplayRecords()
            });
            $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                if (sortColumns.length == 0) {
                    paginationOptions.iSortCols = null;
                } else {
                    paginationOptions.iSortCols = " Order By " + sortColumns[0].field + " " + sortColumns[0].sort.direction;
                }
                getDisplayRecords();
            });
            gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                paginationOptions.pageNumber = newPage;
                paginationOptions.iDisplayLength = pageSize;
                getDisplayRecords();
            });
            //$interval(function () {
            //    $scope.gridApi.core.handleWindowResize();
            //}, 500, 10);
            //gridApi.core.on.renderingComplete($scope, function () {
            //    if (CallBackParam != 0) {
            //        $scope.CallBackParam();
            //    }
            //});
            //gridApi.selection.on.rowSelectionChanged($scope, function (row) {

            //    if (CallonRowSelect != 0) {
            //        $scope.CurrentRowSelect(row);
            //    }

            //});

            //gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            //    $scope.selected(rows);
            //});
        }
    };
    var getDisplayRecords = function () {
        var url = URL;//'@Url.Action("GetAllData", "Utility")';
        var data = {
            pageNumber: paginationOptions.pageNumber,
            iDisplayLength: paginationOptions.iDisplayLength,
            iDisplayStart: (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength,
            sSearch: sSearch,
            iSortCols: paginationOptions.iSortCols,
        };
        for (var i = 0; i < Paramdata.length; i++) {
            var obj = Paramdata[i];
            for (var key in obj) {
                var ParamName = key;
                var ParamValue = obj[key];
                data[ParamName] = ParamValue
            }
        }

        var config = {
            params: data,
            headers: { 'Accept': 'application/json' }
        };
        $http.get(url, config).success(function (data) {
            $scope[GridOptionName].totalItems = data.iTotalDisplayRecords;
            paginationOptions.iDisplayStart = (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength;
            $scope[GridOptionName].data = data.aaData;
            $scope.DefaultAll = data;

            $scope.height = (($scope[GridOptionName].data.length * 30) + 30);
            $scope.height += 80;
            loaderHide();
        });
    };

    getDisplayRecords();
}

// For ManufacturingMemoIssue.cshtml Page
function GriedParamPush($scope, $http, $q, $interval, uiGridConstants, URL, header, GridOptionName) {
    var paginationOptions = {
        pageNumber: 1,
        iDisplayStart: 0,
        iDisplayLength: 25,
        sSearch: sSearch,
        iSortCols: null
    };
    $scope[GridOptionName] = {
        paginationPageSizes: [5, 25, 50, 75],
        paginationPageSize: paginationOptions.iDisplayLength,
        useExternalPagination: true,
        useExternalSorting: true,
        useExternalFiltering: true,
        enableColumnResizing: true,
        columnDefs:
           header
        ,
        /* START Grid Option  */
        enableGridMenu: true,
        // enableSelectAll: true,
        // exporterMenuPdf: false, // ADD THIS
        exporterCsvFilename: 'myFile.csv',
        exporterPdfDefaultStyle: { fontSize: 9 },
        exporterPdfTableStyle: { margin: [30, 30, 30, 30] },
        exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'red' },
        exporterPdfHeader: { text: "My Header", style: 'headerStyle' },

        exporterPdfFooter: function (currentPage, pageCount) {
            return { text: currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
        },
        exporterPdfCustomFormatter: function (docDefinition) {
            docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
            docDefinition.styles.footerStyle = { fontSize: 10, bold: true };
            return docDefinition;
        },
        exporterPdfOrientation: 'portrait',
        exporterPdfPageSize: 'A4',
        exporterPdfMaxGridWidth: 500,
        exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
        /* END Grid Option  */
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.gridApiParam = gridApi;
            $scope.gridApi.core.on.filterChanged($scope, function () {
                var grid = this.grid;
                paginationOptions.sSearch = "";
                for (var i = 0; i < grid.columns.length; i++) {
                    if (grid.columns[i].cellClass == "InQueryInt") {
                        var searchTmp = grid.columns[i].filters[0].term.trim().replace(/,\s*$/, "");
                        if (searchTmp != "") {
                            paginationOptions.sSearch += " AND " + grid.columns[i].colDef["field"] + " in (" + searchTmp + ")";
                        }
                        else {
                            paginationOptions.sSearch = "";
                        }
                    }
                    else {
                        paginationOptions.sSearch += " AND " + grid.columns[i].colDef["field"] + " like '%" + grid.columns[i].filters[0].term + "%'";
                    }
                }
                getDisplayRecords()
            });
            $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                if (sortColumns.length == 0) {
                    paginationOptions.iSortCols = null;
                } else {
                    paginationOptions.iSortCols = " Order By " + sortColumns[0].field + " " + sortColumns[0].sort.direction;
                }
                getDisplayRecords();
            });
            gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                paginationOptions.pageNumber = newPage;
                paginationOptions.iDisplayLength = pageSize;
                getDisplayRecords();
            });

        }
    };
    var getDisplayRecords = function () {
        var url = URL;//'@Url.Action("GetAllData", "Utility")';
        var data = {
            pageNumber: paginationOptions.pageNumber,
            iDisplayLength: paginationOptions.iDisplayLength,
            iDisplayStart: (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength,
            sSearch: sSearch,
            iSortCols: paginationOptions.iSortCols,
        };
        for (var i = 0; i < Paramdata.length; i++) {
            var obj = Paramdata[i];
            for (var key in obj) {
                var ParamName = key;
                var ParamValue = obj[key];
                data[ParamName] = ParamValue
            }
        }

        var config = {
            params: data,
            headers: { 'Accept': 'application/json' }
        };
        $http.get(url, config).success(function (data) {
            if (data.totalNotRecords != "") {
                toastr["info"]("Record Not Found : 0 ")
            }

            $scope[GridOptionName].totalItems = data.iTotalDisplayRecords;
            paginationOptions.iDisplayStart = (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength;

            if (data.aaData.length > 0) {

                jQuery.each($scope.data, function (i, val) {
                    var PacketCreateId = data.aaData[i].Packet_Create_Id
                    if (val.Packet_Create_Id == PacketCreateId) // delete index
                    {
                        delete $scope.data[i];
                    }
                });
            }
            data.aaData.forEach(function (row, index) {
                $scope.data.push(row);
            });
            $scope[GridOptionName].data = $scope.data;
            $scope.height = (($scope[GridOptionName].data.length * 30) + 30);
            $scope.height += 80;

        });
    };

    getDisplayRecords();
}

var reSize = function (rows) {
    // This will adjust the css after the Data is loaded
    var newHeight = (rows * 30) + 60;
    angular.element(document.getElementsByClassName('grid')[0]).css('height', newHeight + 'px');

};


function GriedAddRow($scope) {

    var filed = {};
    if (!$scope.rowCol) {
        $scope.gridOptions.columnDefs.forEach(function (h) {
            if (h.field == "RowInfo") {
                filed[h.field] = 'NEW'
            }
            else {
                filed[h.field] = '';
            }
        });
        $scope.gridOptions.data.push(filed)
        return
    }

    var index = $scope.gridOptions.data.indexOf($scope.rowCol.row.entity);
    if (index < 0) {
    }
    $scope.gridOptions.columnDefs.forEach(function (h) {
        if (h.field == "RowInfo") {
            filed[h.field] = 'NEW'
        }
        else {
            filed[h.field] = '';
        }
    });
    $scope.gridOptions.data.splice(index, 0, filed);


    //  $scope.gridOptions.data.push(filed)//last row insert as new edit mode
    //$scope.gridOptions.data.unshift(filed);//first row insert as new edit mode
}


function GriedAddRowOption($scope, GridOptionName) {
    var filed = {};
    $scope[GridOptionName].columnDefs.forEach(function (h) {
        if (h.field == "RowInfo") {
            filed[h.field] = 'NEW'
        }
        else {
            filed[h.field] = '';
        }
    });
    $scope[GridOptionName].data.push(filed)

}

function ValidateRequired(ValGroup) {
    var retVal = false;
    $('.wrapper :input:visible[required="required"]').each(function () {
        if ($(this).attr("ngvalgroup") == ValGroup) {

            if (!this.validity.valid) {
                $(this).focus();
                // break
                if (!$(this).attr("ngMessage") == false) {
                    validationMessage($(this).attr("ngMessage"))

                }
                retVal = false;
                return false;
            }
            else {
                retVal = true;
            }
        }
    });

    return retVal;

}

function CheckRequired(ValGroup) {
    var retVal = false;
    $('.wrapper :input:visible[required="required"]').each(function () {
        if ($(this).attr("ngvalgroup") == ValGroup) {
            if ($(this).val().trim() == "") {
                $(this).focus();
                if (!$(this).attr("ngMessage") == false) {
                    validationMessage($(this).attr("ngMessage"))
                }
                retVal = false;
                return false;
            }
            else {

                retVal = true;
            }
        }
    });

    return retVal;

}

function ValidateRequired11(ValGroup) {
    var retVal = true;

    $('.wrapper :input:visible[required="required"]').each(function () {
        if ($(this).val().trim() == "") {
            $(this).focus();
            if (!$(this).attr("ngMessage") == false) {
                validationMessage($(this).attr("ngMessage"))
            }
            retVal = false;
        }
    });
    return retVal;

}

function ValidateRequired12() {
    var retVal = true;
    $('.required').each(function () {
        if ($(this).val().trim() == "") {
            $(this).focus();
            if (!$(this).attr("ngMessage") == false) {
                validationMessage($(this).attr("ngMessage"))
            }
            retVal = false;
        }
    });
    return retVal;
}

// Clipboard grid
module.directive('uiGridCellSelection', function ($compile) {
    /*
    Proof of concept, in reality we need on/off events etc.
    */
    return {
        require: 'uiGrid',
        link: function (scope, element, attrs, uiGridCtrl) {
            // Taken from cellNav
            //add an element with no dimensions that can be used to set focus and capture keystrokes
            var gridApi = uiGridCtrl.grid.api
            var focuser = $compile('<div class="ui-grid-focuser" tabindex="-1"></div>')(scope);
            element.append(focuser);

            uiGridCtrl.focus = function () {
                focuser[0].focus();
            };


            gridApi.cellNav.on.viewPortKeyDown(scope, function (e) {
                if ((e.keyCode === 99 || e.keyCode === 67) && e.ctrlKey) {
                    var cells = gridApi.cellNav.getCurrentSelection();
                    var copyString = '',
                     rowId = cells[0].row.uid;
                    angular.forEach(cells, function (cell) {
                        if (cell.row.uid !== rowId) {
                            copyString += '\n';
                            rowId = cell.row.uid;
                        }
                        copyString += gridApi.grid.getCellValue(cell.row, cell.col).toString();
                        copyString += ', ';

                    })
                    // Yes, this should be build into a directive, but this is a quick and dirty example.
                    var textArea = document.getElementById("grid-clipboard");
                    textArea.value = copyString;
                    textArea = document.getElementById("grid-clipboard").select();
                }
            })
            focuser.on('keyup', function (e) {

            })
        }
    }
}).directive('uiGridClipboard', function () {
    return {
        template: '<textarea id="grid-clipboard" ng-model="uiGridClipBoardContents"></textarea>',
        replace: true,
        link: function (scope, element, attrs) {
            // Obviously this needs to be hidden better (probably a z-index, and positioned behind something opaque)
            element.css('height', '1px');
            element.css('width', '1px');
            element.css('resize', 'none');
        }
    };
});

/*End stable version */


//function ValidateRequired1() {

//    var retVal = false;
//    $('.wrapper .st :input:visible[required="required"]').each(function () {
//        if (!this.validity.valid) {
//            $(this).focus();
//            // break
//            if (!$(this).attr("ngMessage") == false) {
//                validationMessage($(this).attr("ngMessage"))
//                // toaster_message('error', 'Warning', $(this).attr("ngMessage"));
//            }
//            retVal = false;
//            return false;
//        }
//        else {
//            retVal = true;
//        }
//    });

//    return retVal;

//}
module.directive('valNum', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {


                if (angular.isUndefined(val)) {
                    var val = '';
                }

                if (val == ".") {
                    ngModelCtrl.$setViewValue("");
                    ngModelCtrl.$render();
                    return "";
                }
                if (val >= 100) {
                    ngModelCtrl.$setViewValue("100");
                    ngModelCtrl.$render();
                    return '100'
                }

                var clean = val.replace(/[^0-9\.]/g, '');
                var negativeCheck = clean.split('-');
                var decimalCheck = clean.split('.');
                if (!angular.isUndefined(negativeCheck[1])) {
                    negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                    clean = negativeCheck[0] + '-' + negativeCheck[1];
                    if (negativeCheck[0].length > 0) {
                        clean = negativeCheck[0];
                    }

                }

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 2);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
        }
    };
});
module.directive("decimals", function ($filter) {
    // Example : <input type="text" class="form-control input-sm" ng-model="PartyMaster.discountdifference" decimals="2" decimal-point=".">
    return {
        restrict: "A", // Only usable as an attribute of another HTML element
        require: "?ngModel",
        scope: {
            decimals: "@",
            decimalPoint: "@"
        },
        link: function (scope, element, attr, ngModel) {
            var decimalCount = parseInt(scope.decimals) || 2;
            var decimalPoint = scope.decimalPoint || ".";
            // Run when the model is first rendered and when the model is changed from code
            ngModel.$render = function () {
                if (ngModel.$modelValue != null && ngModel.$modelValue >= 0) {
                    if (typeof decimalCount === "number") {
                        element.val(ngModel.$modelValue.toFixed(decimalCount).toString().replace(".", "."));
                        //element.val(ngModel.$modelValue.toFixed(decimalCount).toString().replace(".", ","));
                    } else {
                        element.val(ngModel.$modelValue.toString().replace(".", "."));
                        //element.val(ngModel.$modelValue.toString().replace(".", ","));
                    }
                }
            }

            // Run when the view value changes - after each keypress
            // The returned value is then written to the model
            ngModel.$parsers.unshift(function (newValue) {
                if (typeof decimalCount === "number") {
                    var floatValue = parseFloat(newValue.replace(",", "."));
                    if (decimalCount === 0) {
                        return parseInt(floatValue);
                    }
                    return parseFloat(floatValue.toFixed(decimalCount));
                }

                return parseFloat(newValue.replace(",", "."));
            });

            // Formats the displayed value when the input field loses focus
            element.on("change", function (e) {
                var floatValue = parseFloat(element.val().replace(",", "."));
                if (!isNaN(floatValue) && typeof decimalCount === "number") {
                    if (decimalCount === 0) {
                        element.val(parseInt(floatValue));
                    } else {
                        var strValue = floatValue.toFixed(decimalCount);
                        element.val(strValue.replace(".", decimalPoint));
                    }
                }
            });
        }
    }
});

module.directive('onlyDigits', function () {
    return {
        require: '?ngModel',
        restrict: 'A',
        link: function (scope, element, attr, ctrl) {
            function inputValue(val) {
                if (val) {
                    var digits = val.replace(/[^0-9]/g, '');

                    if (digits !== val) {
                        ctrl.$setViewValue(digits);
                        ctrl.$render();
                    }
                    return parseInt(digits, 10);
                }
                return undefined;
            }
            ctrl.$parsers.push(inputValue);

        }
    };
});

module.directive('onlyTexts', function () {
    return {
        require: '?ngModel',
        restrict: 'A',
        link: function (scope, element, attr, ctrl) {
            function inputValue(val) {
                if (val) {
                    var Texts = val.replace(/[^a-zA-Z ]/g, '');

                    if (Texts !== val) {
                        ctrl.$setViewValue(Texts);
                        ctrl.$render();
                    }
                    return Texts;
                }
                return undefined;
            }
            ctrl.$parsers.push(inputValue);

        }
    };
});
module.directive('validNumber', function () {

    //This module Support Property :  (1:valid-number) (2:allow-decimal="false/True") (3:allow-negative="false/True") (4:decimal-upto="1/2/3/ upto 100....")
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            element.on('keydown', function (event) {
                var keyCode = []
                if (attrs.allowNegative == "true") {
                    keyCode = [8, 9, 36, 35, 37, 39, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 109, 110, 173, 190, 189];
                }
                else {
                    var keyCode = [8, 9, 36, 35, 37, 39, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 110, 173, 190];
                }


                if (attrs.allowDecimal == "false") {

                    var index = keyCode.indexOf(190);


                    if (index > -1) {
                        keyCode.splice(index, 1);
                    }

                }
                if ($.inArray(event.which, keyCode) == -1) event.preventDefault();
                else {
                    //console.log(2);
                    var oVal = ngModelCtrl.$modelValue || '';
                    if ($.inArray(event.which, [109, 173]) > -1 && oVal.indexOf('-') > -1) event.preventDefault();
                    else if ($.inArray(event.which, [110, 190]) > -1 && oVal.indexOf('.') > -1) event.preventDefault();
                }
            })
            .on('blur', function () {

                if (element.val() == '' || parseFloat(element.val()) == 0.0 || element.val() == '-') {
                    ngModelCtrl.$setViewValue('0.000');
                }
                else if (attrs.allowDecimal == "false") {
                    ngModelCtrl.$setViewValue(element.val());
                }
                else {
                    if (attrs.decimalUpto) {
                        var fixedValue = parseFloat(element.val()).toFixed(attrs.decimalUpto);
                    }
                    else { var fixedValue = parseFloat(element.val()).toFixed(3); }
                    ngModelCtrl.$setViewValue(fixedValue);
                }



                ngModelCtrl.$render();
                scope.$apply();
            });

            ngModelCtrl.$parsers.push(function (text) {
                var oVal = ngModelCtrl.$modelValue;
                var nVal = ngModelCtrl.$viewValue;
                //console.log(nVal);
                if (parseFloat(nVal) != nVal) {

                    if (nVal === null || nVal === undefined || nVal == '' || nVal == '-') oVal = nVal;

                    ngModelCtrl.$setViewValue(oVal);
                    ngModelCtrl.$render();

                    return oVal;
                }
                else {
                    var decimalCheck = nVal.split('.');
                    if (!angular.isUndefined(decimalCheck[1])) {
                        if (attrs.decimalUpto)
                            decimalCheck[1] = decimalCheck[1].slice(0, attrs.decimalUpto);
                        else
                            decimalCheck[1] = decimalCheck[1].slice(0, 3);
                        nVal = decimalCheck[0] + '.' + decimalCheck[1];
                    }

                    ngModelCtrl.$setViewValue(nVal);
                    ngModelCtrl.$render();
                    return nVal;
                }
            });

            ngModelCtrl.$formatters.push(function (text) {
                if (text == '0' || text == null && attrs.allowDecimal == "false") {
                    ngModelCtrl.$setViewValue("0");
                    return 0;
                }
                else if (text == '0' || text == null && attrs.allowDecimal != "false" && attrs.decimalUpto == undefined) {
                    ngModelCtrl.$setViewValue("0.000");
                    return '0.000';
                }
                else if (text == '0' || text == null && attrs.allowDecimal != "false" && attrs.decimalUpto != undefined) {
                    ngModelCtrl.$setViewValue(parseFloat(0).toFixed(attrs.decimalUpto));
                    return parseFloat(0).toFixed(attrs.decimalUpto);
                }
                else if (attrs.allowDecimal != "false" && attrs.decimalUpto != undefined) {
                    ngModelCtrl.$setViewValue(parseFloat(text).toFixed(attrs.decimalUpto));
                    return parseFloat(text).toFixed(attrs.decimalUpto);
                }
                else {
                    ngModelCtrl.$setViewValue(parseFloat(text).toFixed(3));
                    return parseFloat(text).toFixed(3);
                }
            });

        }
    };
});

module.directive('validNumber1', function () {

    //This module Support Property :  (1:valid-number) (2:allow-decimal="false/True") (3:allow-negative="false/True") (4:decimal-upto="1/2/3/ upto 100....")
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            element.on('keydown', function (event) {
                var keyCode = []
                if (attrs.allowNegative == "true") {
                    keyCode = [8, 9, 36, 35, 37, 39, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 109, 110, 173, 190, 189];
                }
                else {
                    keyCode = [8, 9, 36, 35, 37, 39, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 110, 173, 190];
                }


                if (attrs.allowDecimal == "false") {

                    var index = keyCode.indexOf(190);


                    if (index > -1) {
                        keyCode.splice(index, 1);
                    }

                }
                //alert(keyCode);
                if ($.inArray(event.which, keyCode) == -1) event.preventDefault();
                else {
                    //console.log(2);
                    var oVal = ngModelCtrl.$modelValue || '';
                    if ($.inArray(event.which, [109, 173]) > -1 && oVal.indexOf('-') > -1)
                        event.preventDefault();
                    else if ($.inArray(event.which, [110, 190]) > -1 && oVal.indexOf('.') > -1)
                        event.preventDefault();
                }
            })
              .on('blur', function () {

                  if (element.val() == '' || parseFloat(element.val()) == 0.0 || element.val() == '-') {
                      ngModelCtrl.$setViewValue('');
                  }
                  else if (attrs.allowDecimal == "false") {
                      ngModelCtrl.$setViewValue(element.val());
                  }
                  else {
                      if (attrs.decimalUpto) {
                          var fixedValue = parseFloat(element.val()).toFixed(attrs.decimalUpto);
                      }
                      else { var fixedValue = parseFloat(element.val()).toFixed(3); }
                      ngModelCtrl.$setViewValue(fixedValue);
                  }



                  ngModelCtrl.$render();
                  scope.$apply();
              });

            ngModelCtrl.$parsers.push(function (text) {
                var oVal = ngModelCtrl.$modelValue;
                var nVal = ngModelCtrl.$viewValue;
                //console.log(nVal);
                /* if (parseFloat(nVal) != nVal) {
 
                     if (nVal === null || nVal === undefined || nVal == '' || nVal == '-') oVal = nVal;
 
                     ngModelCtrl.$setViewValue(oVal);
                     ngModelCtrl.$render();
 
                     return oVal;
                 }
                 else 
                 */
                //{
                var decimalCheck = nVal.split('.');
                if (!angular.isUndefined(decimalCheck[1])) {
                    if (attrs.decimalUpto)
                        decimalCheck[1] = decimalCheck[1].slice(0, attrs.decimalUpto);
                    else
                        decimalCheck[1] = decimalCheck[1].slice(0, 3);
                    nVal = decimalCheck[0] + '.' + decimalCheck[1];
                }
                // }
                ngModelCtrl.$setViewValue(nVal);
                ngModelCtrl.$render();
                return nVal;

            });

            ngModelCtrl.$formatters.push(function (text) {
                if (text == '0' || text == null && attrs.allowDecimal == "false") {
                    ngModelCtrl.$setViewValue("0");
                    return 0;
                }
                else if (text == '0' || text == null && attrs.allowDecimal != "false" && attrs.decimalUpto == undefined) {
                    ngModelCtrl.$setViewValue("");
                    return '';
                }
                else if (text == '0' || text == null && attrs.allowDecimal != "false" && attrs.decimalUpto != undefined) {
                    ngModelCtrl.$setViewValue(parseFloat(0).toFixed(attrs.decimalUpto));
                    return parseFloat(0).toFixed(attrs.decimalUpto);
                }
                else if (attrs.allowDecimal != "false" && attrs.decimalUpto != undefined) {
                    ngModelCtrl.$setViewValue(parseFloat(text).toFixed(attrs.decimalUpto));
                    return parseFloat(text).toFixed(attrs.decimalUpto);
                }
                else {
                    ngModelCtrl.$setViewValue(parseFloat(text).toFixed(3));
                    return parseFloat(text).toFixed(3);
                }
            });

        }
    };
});
// For session expire handle
module.factory('httpAuthInterceptor', function ($q, $window) {
    return {
        'responseError': function (response) {
            // NOTE: detect error because of unauthenticated user
            if ([401, 403].indexOf(response.status) >= 0) {
                // redirecting to login page  
                Noty("error", "oops ! your session is expire.");
                var url = '/Login';
                setTimeout(function () { $window.location.href = url }, 700);
                return $q.reject(rejection);
            } else {
                return $q.reject(rejection);
            }
        }
    };
});
module.config(function ($httpProvider) {
    $httpProvider.interceptors.push('httpAuthInterceptor');
});

module.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
}]);
//END
module.directive('validatealphanumeric', function () {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            elm.on('keydown', function (event) {
                var $input = $(this);
                var value = $input.val();
                value = value.replace(/[^a-zA-Z0-9]/g, '')
                $input.val(value);
                if (event.which == 64 || event.which == 16) {
                    // to allow numbers  
                    return false;
                } else if (event.which >= 48 && event.which <= 57) {
                    // to allow numbers  
                    return true;
                }
                else if (event.which >= 65 && event.which <= 90) {
                    // to alpah capital numbers  
                    return true;
                } else if (event.which >= 97 && event.which <= 172) {
                    // to alpah alpha numbers  
                    return true;
                }
                else if (event.which >= 96 && event.which <= 105) {
                    // to allow numpad number  
                    return true;
                } else if ([8, 9, 13, 27, 37, 38, 39, 40].indexOf(event.which) > -1) {
                    // to allow backspace, enter, escape, arrows  
                    return true;
                } else {
                    event.preventDefault();
                    // to stop others  
                    //alert("Sorry Only Numbers Allowed");  
                    return false;
                }
            }
            );
        }
    }
});

module.directive('setDefaultvalue', function () {
    //Example <input type="text" class="form-control input-sm" ng-model="Test" set-Defaultvalue defalut-Value="123456">
    //Example <select class="form-control input-sm" ng-model="PartyMaster.BusinessTypeID" style="width: 100%;" ng-options="item.Business_Type_Id as item.BussinessName for item in BusinessTypes"  set-Defaultvalue defalut-Value="2">
    //Example <input type="checkbox" value="1" ng-model="PartyMaster.allowonlinebuy" set-Defaultvalue defalut-Value="0"> Allow&nbsp;onlineBuy

    return {
        require: '?ngModel',
        scope: {
            defalutValue: "@"
        },
        restrict: 'A',
        link: function (scope, element, attr, ctrl) {
            var GetDefaultVal = parseInt(scope.defalutValue);
            if (!ctrl.$modelValue) {
                //ctrl.$formatters.push(GetDefaultVal)
                ctrl.$setViewValue(GetDefaultVal);
                ctrl.$render();
            }
        }
    };
});

module.filter('myDate', function ($filter) {
    return function (theDate) {
        return $filter('date')(DotnetTojson(theDate), "dd/MMM/yyyy");
    }
});
module.directive('chosen', function () {
    var linker = function (scope, element, attrs) {
        var list = attrs['chosen'];

        scope.$watch(list, function () {
            element.trigger('chosen:updated');
        });

        scope.$watch(attrs['ngModel'], function () {
            element.trigger('chosen:updated');
        });

        element.chosen();
    };

    return {
        restrict: 'A',
        link: linker
    };
});

module.directive('superColWidthUpdate', ['$timeout', function ($timeout) {
    return {
        'restrict': 'A',
        'link': function (scope, element) {
            var _colId = scope.col.colDef.superCol,
                _el = jQuery(element);
            _el.on('resize', function () {
                _updateSuperColWidth();
            });
            var _updateSuperColWidth = function () {
                $timeout(function () {
                    var _parentCol = jQuery('.ui-grid-header-cell[col-name="' + _colId + '"]');
                    var _parentWidth = _parentCol.outerWidth(),
                        _width = _el.outerWidth();

                    if (_parentWidth + 1 >= _width) {
                        _parentWidth = _parentWidth + _width;
                    } else {
                        _parentWidth = _width;
                    }

                    _parentCol.css({
                        'min-width': _parentWidth + 'px',
                        'max-width': _parentWidth + 'px',
                        'text-align': "center"
                    });
                }, 0);
            };
            _updateSuperColWidth();
        }
    };
}]);
module.filter('unique', function () {

    return function (items, filterOn) {

        if (filterOn === false) {
            return items;
        }

        if ((filterOn || angular.isUndefined(filterOn)) && angular.isArray(items)) {
            var hashCheck = {}, newItems = [];

            var extractValueToCompare = function (item) {
                if (angular.isObject(item) && angular.isString(filterOn)) {
                    return item[filterOn];
                } else {
                    return item;
                }
            };

            angular.forEach(items, function (item) {
                var valueToCheck, isDuplicate = false;

                for (var i = 0; i < newItems.length; i++) {
                    if (angular.equals(extractValueToCompare(newItems[i]), extractValueToCompare(item))) {
                        isDuplicate = true;
                        break;
                    }
                }
                if (!isDuplicate) {
                    newItems.push(item);
                }

            });
            items = newItems;
        }
        return items;
    };
});
module.filter('sumOfFloatValue', function () {
    return function (data, key) {

        if (angular.isUndefined(data) || angular.isUndefined(key))
            return 0;
        var sum = 0;
        angular.forEach(data, function (v, k) {
           
            sum = sum + parseFloat(v[key.key]);
        });
        return sum;
    }
});

module.filter('sumOfIntValue', function () {
    return function (data, key) {

        if (angular.isUndefined(data) || angular.isUndefined(key))
            return 0;
        var sum = 0;
        angular.forEach(data, function (v, k) {
            sum = sum + parseInt(v[key.key]);
        });
        return sum;
    }
});

//Created By Ashish
module.filter('sumOfFValue', function () {
    return function (data, key) {

        if (angular.isUndefined(data) || angular.isUndefined(key))
            return 0;
        var sum = 0;
        angular.forEach(data, function (v, k) {

            sum = sum + parseFloat(v[key]);
        });
        return sum;
    }
});

module.directive('decimalNumber', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {
             
                if (angular.isUndefined(val)) {
                    var val = '';
                }

                var clean = val.replace(/[^-0-9\.]/g, '');
                var negativeCheck = clean.split('-');
                var decimalCheck = clean.split('.');
                if (!angular.isUndefined(negativeCheck[1])) {
                    negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                    clean = negativeCheck[0] + '-' + negativeCheck[1];
                    if (negativeCheck[0].length > 0) {
                        clean = negativeCheck[0];
                    }

                }

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 3);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
               
                if (event.keyCode === 32 ) {
                    event.preventDefault();
                }
            });
        }
    };
});


module.filter('groupBy', function () {
    return function (data, key) {
        if (!(data && key)) return;
        var result = {};
        for (var i = 0; i < data.length; i++) {
            if (!result[data[i][key]])
                result[data[i][key]] = [];
            result[data[i][key]].push(data[i])
        }
        return result;
    };
});


//module.filter('groupBy', ['$parse', function ($parse) {
//    return function (list, group_by) {

//        var filtered = [];
//        var prev_item = null;
//        var group_changed = false;
//        // this is a new field which is added to each item where we append "_CHANGED"
//        // to indicate a field change in the list
//        //was var new_field = group_by + '_CHANGED'; - JB 12/17/2013
//        var new_field = 'group_by_CHANGED';

//        // loop through each item in the list
//        angular.forEach(list, function (item) {

//            group_changed = false;

//            // if not the first item
//            if (prev_item !== null) {

//                // check if any of the group by field changed

//                //force group_by into Array
//                group_by = angular.isArray(group_by) ? group_by : [group_by];

//                //check each group by parameter
//                for (var i = 0, len = group_by.length; i < len; i++) {
//                    if ($parse(group_by[i])(prev_item) !== $parse(group_by[i])(item)) {
//                        group_changed = true;
//                    }
//                }


//            }// otherwise we have the first item in the list which is new
//            else {
//                group_changed = true;
//            }

//            // if the group changed, then add a new field to the item
//            // to indicate this
//            if (group_changed) {
//                item[new_field] = true;
//            } else {
//                item[new_field] = false;
//            }

//            filtered.push(item);
//            prev_item = item;

//        });

//        return filtered;
//    };
//}]);


/* start javascript function*/
function removeA(arr) {
    var what, a = arguments, L = a.length, ax;
    while (L > 1 && arr.length) {
        what = a[--L];
        while ((ax = arr.indexOf(what)) !== -1) {
            arr.splice(ax, 1);
        }
    }
    return arr;
}


function SetProperString(d) {

    if (d == null) return '';
    return (d.length ? "'" + d.join("','") + "'" : "");
}


/*  Push Json Object Function Start */

/* Example Start

    json1 = [{id:2, name: 'xyz'}],
    json2 = [{ id: 3, name: 'xyy' }],
    $.concat(json1, json2);

   Example End */

(function ($) { $.concat || $.extend({ concat: function (b, c) { var a = []; for (var x in arguments) if (typeof a == 'object') a = a.concat(arguments[x]); return a } }); })(jQuery);

/*  Push Json Object Function End */


/*
    Json Where Query Angularjs
    
    jsondata = [{id:2, name: 'xyz'}],

    var data = $filter('filter')(jsondata, function (o) {
        return o.id == 2 || o.id == 3;
    });

*/



















































































/*Start Beta version */



function TempGriedAddRow($scope) {
    var filed = {};
    $scope.gridOptions.columnDefs.forEach(function (h) {
        filed[h.field] = ''
    });
    $scope.gridOptions.data.push(filed)
}


function TempGriedFill($scope, $http, $q, $interval, uiGridConstants, URL, header, SavePostDataURL, GridOptionName) {
    var paginationOptions = {
        pageNumber: 1,
        iDisplayStart: 0,
        iDisplayLength: 25,
        sSearch: sSearch,
        iSortCols: null
    };
    $scope.RunTimeChecked = false;

    $scope[GridOptionName] = {
        paginationPageSizes: [5, 25, 50, 75],
        paginationPageSize: paginationOptions.iDisplayLength,
        useExternalPagination: true,
        useExternalSorting: true,
        //  enableFiltering: true,
        useExternalFiltering: true,
        enableColumnResizing: true,
        //  showColumnFooter: true,
        columnDefs:
           header
        ,
        /* START Grid Option  */
        enableGridMenu: true,
        // enableSelectAll: true,
        // exporterMenuPdf: false, // ADD THIS
        exporterCsvFilename: 'myFile.csv',
        exporterPdfDefaultStyle: { fontSize: 9 },
        exporterPdfTableStyle: { margin: [30, 30, 30, 30] },
        exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'red' },
        exporterPdfHeader: { text: "My Header", style: 'headerStyle' },

        exporterPdfFooter: function (currentPage, pageCount) {
            return { text: currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
        },
        exporterPdfCustomFormatter: function (docDefinition) {
            docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
            docDefinition.styles.footerStyle = { fontSize: 10, bold: true };
            return docDefinition;
        },
        exporterPdfOrientation: 'portrait',
        exporterPdfPageSize: 'A4',
        exporterPdfMaxGridWidth: 500,
        exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),

        /* END Grid Option  */
        onRegisterApi: function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                $scope.$apply();
                $scope.UpdateChangeCalcu(rowEntity);
            });
            $scope.gridApi.core.on.filterChanged($scope, function () {
                var grid = this.grid;
                paginationOptions.sSearch = "";
                for (var i = 0; i < grid.columns.length; i++) {
                    if (grid.columns[i].filters[0].term != undefined) {
                        paginationOptions.sSearch += " AND " + grid.columns[i].colDef["field"] + " like '%" + grid.columns[i].filters[0].term + "%'";
                    }
                }
                getPage()
            });
            $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                if (sortColumns.length == 0) {
                    paginationOptions.iSortCols = null;
                } else {
                    paginationOptions.iSortCols = " Order By " + sortColumns[0].field + " " + sortColumns[0].sort.direction;
                }
                getPage();
            });
            gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                paginationOptions.pageNumber = newPage;
                paginationOptions.iDisplayLength = pageSize;
                getPage();
            });

            gridApi.core.on.renderingComplete($scope, function () {

                if (CallBack != 0) {
                    $scope.CallBack();
                }

            });
        }
    };



    var saveRow = function (rowEntity) {
        // create a fake promise - normally you'd use the promise returned by $http or $resource
        var promise = $q.defer();
        $scope.gridApi.rowEdit.setSavePromise(rowEntity, promise.promise);

        $interval(function () {
            // if ($scope.RunTimeChecked) {
            var response = $http({
                method: "post",
                async: true,
                url: SavePostDataURL,//'@Url.Action("UpdateStoneMaster", "Utility")',
                data: rowEntity,
                dataType: "json"
            });
            //return response;
            //if (rowEntity.gender === 'male') {
            //    promise.reject();
            //} else {
            //    promise.resolve();
            //}
            //}
        }, 3000, 1);
    };
    var getPage = function () {
        var url = URL;//'@Url.Action("GetAllData", "Utility")';

        var data = {
            pageNumber: paginationOptions.pageNumber,
            iDisplayLength: paginationOptions.iDisplayLength,
            iDisplayStart: (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength,
            sSearch: paginationOptions.sSearch,
            iSortCols: paginationOptions.iSortCols
        };

        var config = {
            params: data,
            headers: { 'Accept': 'application/json' }
        };
        $http.get(url, config).success(function (data) {
            $scope[GridOptionName].totalItems = data.iTotalDisplayRecords;
            // var firstRow = (paginationOptions.pageNumber - 1) * paginationOptions.pageSize;//pagesize==iDisplayLength
            paginationOptions.iDisplayStart = (paginationOptions.pageNumber - 1) * paginationOptions.iDisplayLength;

            $scope[GridOptionName].data = data.aaData;
            //$scope.gridOptions.data = data.aaData.slice(firstRow, firstRow + paginationOptions.pageSize);
            // reSize(data.aaData.length);

            //$scope.height = (($scope[GridOptionName].data.length * 30) + 30);

            //$scope.height += 80;


        });

    };

    getPage();
}

function ConvertDate(dateValue) {
    var dateString = dateValue.substr(6);
    var currentTime = new Date(parseInt(dateString));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();

    var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    var date = day + "/" + monthNames[month] + "/" + year;

    return date;
}
function DotnetTojson(DateString) {

    return DateString.replace(/\/Date\((-?\d+)\)\//, '$1');
    // var d = new Date(parseInt(milli));
}
Date.prototype.addDays = function (days) {
    this.setDate(this.getDate() + parseInt(days));
    return this;
};

function fnNumPadLeft(num, places) {

    var zero = places - num.toString().length + 1;
    return Array(+(zero > 0 && zero)).join("0") + num;
}

function QueryStringAshish() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
};






















/*End Beta version */
/*Start  Globally Gried Dropdown*/
//var ParameterToleranceType = [{
//    id: 4,
//    value: 'Value'
//}, {
//    id: 5,
//    value: 'Percentage'
//}];

/* Globally Gried Dropdown*/

/*End Beta version */















/*test question arise on rnd*/
//$scope.gridApi.rowEdit.flushDirtyRows( $scope.gridApi.grid );//check time on update data only get?? //http://plnkr.co/edit/s583DdMHjxkf3yf6Gi7y?p=preview
//easily show popup value http://plnkr.co/edit/D48xcomnXdClccMnl5Jj?p=preview
//datetimepicker used this http://plnkr.co/edit/4mNr86cN6wFOLYQ02QND
//FOR WITHOUT OPTION SELECION GET VALUE GOOD EXAMPLE http://jsfiddle.net/mLrynxh2/2/
//direct se\t http://plnkr.co/edit/x4JAeXra1bP4cQjIBld0?p=preview
//http://plnkr.co/edit/4mNr86cN6wFOLYQ02QND?p=preview

//http://plnkr.co/edit/c0EIopXUgnoNQDyxzbot?p=preview
//aUTOrESIZE
//http://plnkr.co/edit/96o8ZemeM1IG8CpaXwV2?p=preview
//http://plnkr.co/edit/H1B3rKrMM4RaUH5SPVaF?p=preview