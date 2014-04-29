
var moeathomeApp = angular.module('moeathomeApp', ['ngRoute']);

moeathomeApp.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    //$locationProvider.html5Mode(true);
    $routeProvider
        .when('/', {
            templateUrl: '/Home/_HomePage',
        })
        .when('/login', {
            templateUrl: '/Home/_Login',
        })
        .when('/register', {
            templateUrl: '/Home/_Register',
        })
        .when('/blog/:date/:title', {
            templateUrl: '/Home/_ViewBlog',
        })
        .when('/blog/post', {
            templateUrl: '/Home/_PostBlog',
        })
        .otherwise({
            redirectTo: '/'
        });
}]).service('appDataService', ['$rootScope', '$http', function ($rootScope, $http) {
    return new AppDataModel($rootScope, $http);
}]);