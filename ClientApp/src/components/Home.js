import React, { Component } from 'react';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (
            <div>
                <h1>Календарь <span style={{ color: "red" }}>красных</span> дней</h1>
                <p>Здесь вы сможете отмечать начало и конец "красных дней". На основе этих отметок мы будем давать вам прогноз начала следующих)</p>
            </div>
        );
    }
}
