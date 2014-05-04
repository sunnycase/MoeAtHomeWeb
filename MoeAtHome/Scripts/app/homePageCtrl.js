
moeathomeApp.controller('homePageCtrl', ['$scope', '$http', '$sce', 'appDataService', function ($scope, $http, $sce, appDataService) {
    document.title = '首页 - Moe@Home';
    var queryRecentBlogsUrl = "/api/blog/queryRecentBlogs";

    $http({
        method: 'GET',
        url: queryRecentBlogsUrl
    }).success(function (data, status) {
        $scope.blogs = data;
    });

    $scope.renderHtml = function (str) {
        return $sce.trustAsHtml(str);
    };
}]);