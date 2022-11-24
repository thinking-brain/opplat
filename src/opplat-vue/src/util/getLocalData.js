import products from './products.json';
import users from './users.json';
import roles from './roles.json';
import groups from './groups.json';

export default {
    getProducts() {
        return products;
    },
    getUsers() {
        return users;
    },
    getRoles() {
        return roles;
    },
    getGroups() {
        return groups;
    },
};
