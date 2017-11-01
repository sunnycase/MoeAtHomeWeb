import Vue from 'vue';
import { Component } from 'vue-property-decorator';

interface Friend {
    name: string;
    url: string;
}

@Component
export default class AboutMeComponent extends Vue {
    friends: Friend[] = [];

    async mounted() {
        var response = await fetch('api/Friend');
        this.friends = await (response.json() as Promise<Friend[]>);
    }
}