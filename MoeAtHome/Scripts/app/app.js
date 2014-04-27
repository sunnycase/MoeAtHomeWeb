
var moeathomeApp = angular.module('moeathomeApp', ['ngRoute']);

moeathomeApp.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    //$locationProvider.html5Mode(true);
    $routeProvider
        .when('/', {
            templateUrl: '/Home/_HomePage',
        })
        .when('/blog/:date/:title', {
            templateUrl: '/Home/_ViewBlog',
        })
        .otherwise({
            redirectTo: '/'
        });
}]);