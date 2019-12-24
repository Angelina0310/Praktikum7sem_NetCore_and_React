import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService';
import axios from 'axios';

export class KDinfo extends Component {
    static displayName = KDinfo.name;

    constructor(props) {
        super(props);
        this.state = {
            KD: [],
            loading: true,
            isAuthenticated: false,
            userName: null,
            NextKD: null,
        };
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    async populateState() {
        const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
        this.setState({
            isAuthenticated,
            userName: user && user.name
        });
        this.GetLastKD();
        this.GetNextKD()
    }

    static renderLastKD(KD) {
        let text = "";
        let last5 = [];
        let max = 5;
        let last5div = <span></span>;
        if (max > KD.length) {
            max = KD.length;
        }
        if (KD.length > 0) {
            text = "Последний раз \"они\" начались " + KD[0];
            for (let i = 0; i < max; i++) {
                last5.push(KD[i]);
            }
        } else {
            text = "Нет данных о последних месячных!";
        }
        if (KD.length > 0) {
            last5div = <div>
                <h3>Даты начала последних 5 месячных</h3>
                <ul>
                    {last5.map(one =>
                        <li>{one}</li>)}
                </ul>
            </div>;
        }

        return (
            <div>
                <div>
                    <h3>{text}</h3>
                </div>
                {last5div}
            </div>
        );
    }

    handleSubmit = event => {
        event.preventDefault();

        if (this.state.date == null) {
            alert("Введите дату!!!");
            return false;
        }
        this.AddDate();
    }

    onChangeDate(date) {
        this.setState({ date: date.target.value });
    }

    render() {
        const { isAuthenticated, userName } = this.state;
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : KDinfo.renderLastKD(this.state.KD);

        let form = isAuthenticated
            ? <div>
                <h3>Начались...?</h3>
                <form onSubmit={this.handleSubmit}>
                    <p>
                        <label>
                            Когда?
                            <input type="hidden" name="userName" value={userName} />
                            <input type="date" name="date" value={this.state.date} onChange={this.onChangeDate.bind(this)} />
                        </label>
                    </p>
                    <input className="btn btn-success" type="submit" value="ДА!" />
                </form>
            </div>
            : <p></p>
        let nextKDdiv = <div></div>;
        if (this.state.NextKD != null) {
            nextKDdiv = <div>
                    <h3>Когда следующие?</h3>
                    <p>{this.state.NextKD}</p>
                </div>
        }
        return (
            <div>
                <div>
                    <h1>Календарь бесячных</h1>
                </div>

                {form}

                {contents}

                {nextKDdiv}
            </div>
        );
    }

    async GetLastKD() {
        const token = await authService.getAccessToken();
        await fetch('api/days/get?user=' + this.state.userName,
            { headers: !token ? {} : { 'Authorization': `Bearer ${token}` } })
            .then(response => response.json())
            .then(data => this.setState({ KD: data, loading: false }))
            .catch(error => console.log(error));
    }

    async AddDate() {
        const token = await authService.getAccessToken();

        await axios.post('api/days/add', { user: this.state.userName, day: this.state.date }, { headers: !token ? {} : { 'Authorization': `Bearer ${token}` } })
            .then(res => {
                console.log(res);
            })
        this.GetLastKD();
        this.GetNextKD();
    }

    async GetNextKD() {
        const token = await authService.getAccessToken();
        await fetch('api/days/getNextKD?user=' + this.state.userName,
            { headers: !token ? {} : { 'Authorization': `Bearer ${token}` } })
            .then(response => response.json())
            .then(data => this.setState({ NextKD: data }))
            .catch(error => console.log(error));
    }

}
