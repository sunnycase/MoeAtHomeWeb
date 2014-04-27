
moeathomeApp.controller('homePageCtrl', ['$scope', function ($scope) {
    document.title = '首页 - Moe@Home';
    $scope.blogs = [
        {
            'title': '第一篇博文',
            'summary': '博文内容',
            'date': '2014-04-25',
            'tags': ['标签1', '标签2']
        },
        {
            'title': '第二篇博文',
            'summary': '博文内容',
            'date': '2014-04-25',
            'tags': ['标签1']
        }
    ];
}]);