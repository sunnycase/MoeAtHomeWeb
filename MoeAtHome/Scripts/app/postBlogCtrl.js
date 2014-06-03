
moeathomeApp.controller('postBlogCtrl', ['$scope', '$http', '$location', function ($scope, $http, $location) {
    document.title = '发表文章 - Moe@Home';

    var postBlogUrl = 'api/blogs';

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

    //tinymce配置
    $scope.tinymceOptions = {
        //语言
        language: 'zh_CN',
        //拼写检查
        browser_spellcheck: true,
        //插件
        plugins: 'code image link autolink autosave textcolor table preview searchreplace \
                  emoticons visualblocks insertcode',
        //工具栏
        toolbar: [
            'undo redo | formatselect bold italic alignleft aligncenter alignright | forecolor backcolor \
            | searchreplace | link image emoticons insertcode | code preview'
        ],
        tools: 'inserttable',
        table_default_attributes: {
            class: 'ui table segment'
        },
        visualblocks_default_state: true,
        forced_root_block: false,
        height: 300,
    };
}]);