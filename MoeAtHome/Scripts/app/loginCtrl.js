'use strict'
moeathomeApp.controller('loginCtrl', ['$scope', '$http', '$location', function ($scope, $http, $location) {
    var loginUrl = '/Token';

    document.title = '登录 - Moe@Home';
    $scope.enableValidate = false;
    $scope.userName = {
        value: '',
        hasError: function () {
            return $scope.enableValidate && $scope.userName.value == '';
        },
        errorMessage: '用户名不能为空哦。'
    };
    $scope.password = {
        value: '',
        hasError: function () {
            return $scope.enableValidate && $scope.password.value == '';
        },
        errorMessage: '密码也不能为空哦。'
    };
    $scope.rememberMe = false;

    $scope.errors = [];

    $scope.doLogin = function () {
        $scope.errors = [];
        $scope.enableValidate = true;
        if (!$scope.userName.hasError() && !$scope.password.hasError()) {
            $http({
                method: 'POST',
                url: loginUrl,
                data: $.param({
                    grant_type: "password",
                    username: $scope.userName.value,
                    password: $scope.password.value
                }),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).success(function (data, status) {
                if (data.userName && data.access_token) {
                    app.navigateToLoggedIn(data.userName, data.access_token, self.rememberMe());
                    $location.route('/');
                } else {
                    $scope.errors.push('发生了奇怪的问题。');
                }
            }).error(function (data, status) {
                if (data && data.error_description) {
                    $scope.errors.push(data.error_description);
                } else {
                    $scope.errors.push("发生了奇怪的问题。");
                }
            });
        }
    };
}]);