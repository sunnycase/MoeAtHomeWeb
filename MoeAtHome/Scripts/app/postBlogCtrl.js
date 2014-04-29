
moeathomeApp.controller('postBlogCtrl', ['$scope', function ($scope) {
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
        }
    };
}]);