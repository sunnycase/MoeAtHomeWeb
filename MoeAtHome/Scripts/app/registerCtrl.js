'use strict'
moeathomeApp.controller('registerCtrl', ['$scope', '$http', '$location', function ($scope, $http, $location) {
    var registerUrl = '/api/account/register';

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
    $scope.confirmPassword = {
        value: '',
        hasError: function () {
            return $scope.confirmPassword.value != $scope.password.value;
        },
        errorMessage: '确认密码必须和密码相同哦。'
    };
    $scope.email = {
        value: '',
        hasError: function () {
            var reg = /^(\w)+(\.\w+)*@(\w)+((\.\w+)+)$/;
            return $scope.enableValidate && !reg.test($scope.email.value);
        },
        errorMessage: '必须输入正确的 Email 地址哦。'
    };

    $scope.errors = [];

    $scope.doRegister = function () {
        $scope.errors = [];
        $scope.enableValidate = true;

        if ($scope.userName.hasError() || $scope.password.hasError() ||
            $scope.confirmPassword.hasError() || $scope.email.hasError()) return;
        $http({
            method: 'POST',
            url: registerUrl,
            data: $.param({
                username: $scope.userName.value,
                password: $scope.password.value,
                confirmPassword: $scope.confirmPassword.value,
                email: $scope.email.value
            }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).success(function (data, status) {
            $location.path('/login');
        }).error(function (data, status) {
            if (data && data.error_description) {
                $scope.errors.push(data.error_description);
            } else {
                $scope.errors.push("发生了奇怪的问题。");
            }
        });
    };
}]);