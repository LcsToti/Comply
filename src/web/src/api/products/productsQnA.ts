import api from "../axios";

export interface QuestionRequest {
    QuestionText: string;
}
export interface AnswerRequest {
    AnswerText: string;
}

export class ProductQnaApi {

    async addQuestion(productId: string, params: QuestionRequest) {
        const { data } = await api.post(
            `products/${productId}/qna/questions`,
            params
        );
        return data;
    }

    async updateQuestion(productId: string, questionId: string, params: QuestionRequest) {
        const { data } = await api.put(
            `products/${productId}/qna/questions/${questionId}`,
            params
        );
        return data;
    }

    async removeQuestion(productId: string, questionId: string) {
        const { data } = await api.delete(
            `products/${productId}/qna/questions/${questionId}`
        );
        return data;
    }

    async answerQuestion(productId: string, questionId: string, params: AnswerRequest) {
        const { data } = await api.post(
            `products/${productId}/qna/questions/${questionId}/answer`,
            params
        );
        return data;
    }

    async updateAnswer(productId: string, questionId: string, params: AnswerRequest) {
        const { data } = await api.put(
            `products/${productId}/qna/questions/${questionId}/answer`,
            params
        );
        return data;
    }

    async removeAnswer(productId: string, questionId: string) {
        const { data } = await api.delete(
            `products/${productId}/qna/questions/${questionId}/answer`
        );
        return data;
    }
}

export const productQnaApi = new ProductQnaApi();
