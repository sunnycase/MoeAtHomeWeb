'use strict'

function AppDataModel($rootScope) {
    var self = this,
        siteUrl = "/",
        userInfoUrl = "/api/account/userInfo",
        logOffUrl = '/api/account/logout';
    var $http = angular.injector(['moeathomeApp', 'ng']).get('$http');

    self.user = null;
    self.loggedIn = function () { return self.user != null };

    // Other private operations
    function getSecurityHeaders() {
        var accessToken = sessionStorage["accessToken"] || localStorage["accessToken"];

        if (accessToken) {
            return { "Authorization": "Bearer " + accessToken };
        }

        return {};
    }

    self.getUserInfo = function (accessToken) {
        var headers;

        if (typeof (accessToken) !== "undefined") {
            headers = {
                "Authorization": "Bearer " + accessToken
            };
        } else {
            headers = getSecurityHeaders();
        }

        return $http({
            method: 'GET',
            url: userInfoUrl,
            headers: headers
        });
    };

    self.setAccessToken = function (accessToken, persistent) {
        if (persistent) {
            localStorage["accessToken"] = accessToken;
        } else {
            sessionStorage["accessToken"] = accessToken;
        }
    };

    self.clearAccessToken = function () {
        localStorage.removeItem("accessToken");
        sessionStorage.removeItem("accessToken");
    };

    // UI operations
    self.archiveSessionStorageToLocalStorage = function () {
        var backup = {};

        for (var i = 0; i < sessionStorage.length; i++) {
            backup[sessionStorage.key(i)] = sessionStorage[sessionStorage.key(i)];
        }

        localStorage["sessionStorageBackup"] = JSON.stringify(backup);
        sessionStorage.clear();
    };

    self.restoreSessionStorageFromLocalStorage = function () {
        var backupText = localStorage["sessionStorageBackup"],
            backup;

        if (backupText) {
            backup = JSON.parse(backupText);

            for (var key in backup) {
                sessionStorage[key] = backup[key];
            }

            localStorage.removeItem("sessionStorageBackup");
        }
    };

    self.login = function (userName, accessToken, persistent) {
        if (accessToken) {
            self.setAccessToken(accessToken, persistent);
        }
        self.user = new UserInfoModel(userName);
        $rootScope.$broadcast('loggedIn.update');
        $rootScope.$broadcast('user.userName.update');
    };

    self.logOff = function () {
        self.clearAccessToken();
        $http({
            method: 'POST',
            url: logOffUrl
        });
        self.user = null;
        $rootScope.$broadcast('loggedIn.update');
    };

    self.init = function () {
        self.restoreSessionStorageFromLocalStorage();
        self.getUserInfo().success(function (data) {
            if (data.userName) {
                self.login(data.userName);
            } else {
            }
        }).error(function () {
            self.user = null;
            $rootScope.$broadcast('loggedIn.update');
        });
    };
    self.init();
};

moeathomeApp.controller('appDataCtrl', ['$scope', 'appDataService', function ($scope, appDataService) {
    $scope.$on('loggedIn.update', function (event) {
        $scope.loggedIn = appDataService.loggedIn();
        $scope.$apply();
    });
    $scope.loggedIn = appDataService.loggedIn();

    $scope.$on('user.userName.update', function (event) {
        if (appDataService.user != null)
            $scope.userName = appDataService.user.userName;
        else
            $scope.userName = '';
        $scope.$apply();
    });
    $scope.userName = '';
    $scope.logOff = appDataService.logOff;
}]);