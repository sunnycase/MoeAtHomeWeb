
moeathomeApp.controller('postBlogCtrl', ['$scope', '$http', '$location', function ($scope, $http, $location) {
    document.title = '发表文章 - Moe@Home';

    var postBlogUrl = 'api/blog/postBlog';

    $scope.enableValidate = false;

    $scope.title = {
        value: '',
        hasError: function () {
            return $scope.enableValidate && $scope.title.value == '';
        },
        errorMessage: '标题不能为空哦。'
    };

    $scope.content = {
        value: '',
        hasError: function () {
            return $scope.enableValidate && $scope.content.value == '';
        },
        errorMessage: '内容也不能为空哦。'
    };

    $scope.tags = {
        value: '',
        hasError: function () {
            return false;
        },
        errorMessage: '内容也不能为空哦。'
    };

    $scope.errors = [];

    $scope.doPostBlog = function () {
        $scope.errors = [];
        $scope.enableValidate = true;
        if (!$scope.title.hasError() && !$scope.content.hasError()
            && !$scope.tags.hasError()) {
            var tags = $scope.tags.value.split(';');
            if (tags == null) tags = [];
            $http({
                method: 'POST',
                url: postBlogUrl,
                data: $.param({
                    title: $scope.title.value,
                    tags: tags,
                    content: $scope.content.value
                }),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).success(function (data, status) {
                $location.path('/');
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