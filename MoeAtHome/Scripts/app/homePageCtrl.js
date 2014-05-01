
moeathomeApp.controller('homePageCtrl', ['$scope', '$http', 'appDataService', function ($scope, $http, appDataService) {
    document.title = '首页 - Moe@Home';
    var queryRecentBlogsUrl = "/api/blog/queryRecentBlogs";

    $http({
        method: 'GET',
        url: queryRecentBlogsUrl
    }).success(function (data, status) {
        $scope.blogs = data;
    });
}]);