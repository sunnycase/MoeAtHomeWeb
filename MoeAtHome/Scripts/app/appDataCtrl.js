'use strict'

function AppDataModel($rootScope, $http) {
    var self = this,
        siteUrl = "/",
        userInfoUrl = "/api/account/userInfo",
        logOffUrl = '/api/account/logout';

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
        self.autoLogin();
    };

    self.autoLogin = function () {
        self.getUserInfo().success(function (data) {
            if (data.userName) {
                if (self.user == null) {
                    self.login(data.userName);
                }
                self.user.isAdmin = data.isAdministrator;
                $rootScope.$broadcast('user.isAdmin.update');
            } else {
            }
        }).error(function () {
            self.user = null;
            $rootScope.$broadcast('loggedIn.update');
        });
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
        self.autoLogin();
    };
};

moeathomeApp.controller('appDataCtrl', ['$scope', 'appDataService', function ($scope, appDataService) {
    $scope.updateLoggedIn = function () {
        $scope.loggedIn = appDataService.loggedIn();
    };

    $scope.updateUserName = function () {
        if (appDataService.user != null)
            $scope.userName = appDataService.user.userName;
        else
            $scope.userName = '';
    };

    $scope.updateIsAdmin = function () {
        if (appDataService.user != null)
            $scope.isAdmin = appDataService.user.isAdmin;
        else
            $scope.isAdmin = false;
    };

    $scope.$on('loggedIn.update', function (event) {
        $scope.updateLoggedIn();
    });

    $scope.$on('user.userName.update', function (event) {
        $scope.updateUserName();
    });

    $scope.$on('user.isAdmin.update', function (event) {
        $scope.updateIsAdmin();
    });

    $scope.init = appDataService.init;
    $scope.logOff = appDataService.logOff;

    $scope.updateLoggedIn();
    $scope.updateUserName();
    $scope.updateIsAdmin();
}]);