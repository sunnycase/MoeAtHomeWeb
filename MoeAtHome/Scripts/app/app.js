
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
        .otherwise({
            redirectTo: '/'
        });
}]).service('appDataService', ['$rootScope', function ($rootScope) {
    return new AppDataModel($rootScope);
}]);